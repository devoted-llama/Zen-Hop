using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; } = null;
    public Rigidbody2D RigidBody { get; private set; }
    float PowerWithMultiplier { get { return powerAmount * powerForceMultiplier; } }

    [SerializeField]
    readonly float powerForceMultiplier = 1000f;
    float powerAmount = 0;


    readonly int idleHash = Animator.StringToHash("Idle");
    readonly int aimHash = Animator.StringToHash("Aim");
    readonly int crouchHash = Animator.StringToHash("Crouch");
    readonly int jumpHash = Animator.StringToHash("Jump");

    int currentPlatformId = 0;
    Vector3 startPosition;
    float jumpAngle = 0;
    bool doingPlatformActionsCoroutine = false;

    public delegate void PlatformLandedAction(int platformId);
    public event PlatformLandedAction OnPlatformLanded;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        GameStartSetActive();

        startPosition = transform.position;
    }

    public void ResetToStartPosition() {
        transform.position = startPosition;
    }

    void GameStartSetActive() {
        if(GameController.instance.playing == false) {
            gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (GetRigidBodyVelocityGreaterThan(4f)) {
            SetJumpAnimation(true);
        }
    }


    void Start() {
        RigidBody = GetComponent<Rigidbody2D>();
        SetPower(0);

    }

    public void Jump() {
        if (gameObject.activeSelf && GetRigidBodyVelocityEquals(0)) {
            StartCoroutine(JumpCoroutine());
            
        }
    }

    IEnumerator JumpCoroutine() {
        AudioController.instance.PlayJumpSound();
        SetCrouchAnimation(true);
        yield return new WaitForSeconds(0.5f);
        SetCrouchAnimation(false);
        SetJumpAnimation(true);
        RigidBody.AddForce(GetForce());

        SetPower(0);
    }

    public void SetJumpAngle(float angle, float modifier = 0) {
        if (!float.IsNaN(angle)) {
            jumpAngle = modifier + angle;
        }
    }


    Vector2 GetForce() {
        float angle = jumpAngle;

        float ratio = 0f;
        float forceX = 0f;
        float forceY = 0f;

        if (angle >= 0 && angle <= 90) {
            ratio = angle / 90f;
            forceX = ratio * PowerWithMultiplier;
            forceY = (1 - ratio) * PowerWithMultiplier;
        } else if (angle > 90 && angle <= 180) {
            ratio = (angle - 90) / 90f;
            forceX = (1 - ratio) * PowerWithMultiplier;
            forceY = -(ratio) * PowerWithMultiplier;
        } else if (angle > 180 && angle <= 270) {
            ratio = (angle - 180) / 90f;
            forceX = -ratio * PowerWithMultiplier;
            forceY = -(1 - ratio) * PowerWithMultiplier;
        } else if (angle > 270 && angle <= 360) {
            ratio = (angle - 270) / 90f;
            forceX = -(1 - ratio) * PowerWithMultiplier;
            forceY = ratio * PowerWithMultiplier;
        }
        
        Vector2 force = new Vector2(forceX, forceY);

        return force;
    }

    public void SetPower(float power) {
        powerAmount = power > 1 ? 1 : power;
    }


    void OnCollisionEnter2D(Collision2D collision) {

        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null) {
            if (GetRigidBodyVelocityLessThan(4f) && GetJumpAnimation() == true) {
                SetJumpAnimation(false);
            }
            platform.AnimateBounce();
            DoPlatformActions(platform);
        }
    }



    private void OnCollisionStay2D(Collision2D collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null) {
            if (GetRigidBodyVelocityLessThan(4f) && GetJumpAnimation() == true) {
                SetJumpAnimation(false);
            }
        }
    }

    void DoPlatformActions(Platform platform) {
         StartCoroutine(DoPlatformActionsCoroutine(platform));
    }

    bool GetRigidBodyVelocityGreaterThan(float velocity) {
        return RigidBody.velocity.x > velocity || 
            RigidBody.velocity.y > velocity || 
            RigidBody.velocity.x < -velocity || 
            RigidBody.velocity.y < -velocity;
    }

    bool GetRigidBodyVelocityEquals(float velocity) {
        return RigidBody.velocity.x == velocity &&
            RigidBody.velocity.y == velocity;
    }

    bool GetRigidBodyVelocityLessThan(float velocity) {
        return RigidBody.velocity.x < velocity && 
            RigidBody.velocity.y < velocity && 
            RigidBody.velocity.x > -velocity && 
            RigidBody.velocity.y > -velocity;
    }

    bool GetHasLandedOnPlatform() {
        if (Helper.CheckRigidBodyContactsHasComponent<Platform>(RigidBody)) {
            return true;
        }
        return false;
    }

    bool GetHasLandedOnPlatformAndStopped() {
        return GetHasLandedOnPlatform() && GetRigidBodyVelocityLessThan(0.00001f);
    }



    IEnumerator DoPlatformActionsCoroutine(Platform platform) {
       
        if(doingPlatformActionsCoroutine == true) {
            yield break;
        }
        doingPlatformActionsCoroutine = true;

        if (platform.id == currentPlatformId) {
            doingPlatformActionsCoroutine = false;
            yield break;
        }

        yield return new WaitForSecondsRealtime(.5f);

        if (GetHasLandedOnPlatformAndStopped()) {
            currentPlatformId = platform.id;
            OnPlatformLanded(currentPlatformId);
        }
        
        doingPlatformActionsCoroutine = false;
    }


    public void SetAimAnimation(bool value) {
        GetComponent<Animator> ().SetBool (aimHash,value);
    }

    public void SetCrouchAnimation(bool value) {
        GetComponent<Animator> ().SetBool (crouchHash,value);
    }

    public bool GetCrouchAnimation() {
        return GetComponent<Animator>().GetBool(crouchHash);
    }

    public void SetJumpAnimation(bool value) {
        GetComponent<Animator> ().SetBool (jumpHash,value);
    }

    public bool GetJumpAnimation() {
        return GetComponent<Animator>().GetBool(jumpHash);
    }

    public void SetIdleAnimation(bool value) {
        GetComponent<Animator> ().SetBool (idleHash,value);
    }

    IEnumerator RespawnCoroutine() {
        Platform currentPlatform = PlatformController.Instance.GetPlatformById(0);
        Vector3 pos = currentPlatform.transform.position;
        CameraController.instance.MoveTo(currentPlatform.transform);
        yield return new WaitUntil (() => Camera.main.transform.position.x == pos.x);
        pos.y = GameController.instance.respawnHeight;
        Vector2 velocity = new Vector2 (0, -GameController.instance.fallSpeed);
        RigidBody.velocity = velocity;
        transform.position = pos;
    }

    public void Respawn() {
        StartCoroutine(RespawnCoroutine());
    }
}
