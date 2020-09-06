using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Frog : MonoBehaviour {
    public static Frog instance = null;

    public float powerForceMultiplier;
    float powerAmount = 0;
    float powerWithMultiplier { get { return powerAmount * powerForceMultiplier; } }

    int idleHash = Animator.StringToHash("Idle");
    int aimHash = Animator.StringToHash("Aim");
    int crouchHash = Animator.StringToHash("Crouch");
    int jumpHash = Animator.StringToHash("Jump");

    public Button powerButton;


    public Rigidbody2D rigidBody;

    public int currentPlatformId = 0;

    public bool doingPlatformActions = false;

    Vector3 startPosition;

    float jumpAngle;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
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
        if (rigidBody.velocity.x > 4f || rigidBody.velocity.y > 4f || rigidBody.velocity.x < -4f || rigidBody.velocity.y < -4f) {
            SetJump(true);
        }
    }


    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        SetPower(0);

    }

    public void Jump() {
        if (gameObject.activeSelf && rigidBody.velocity.x == 0 && rigidBody.velocity.y == 0) {
            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine() {
        AudioController.instance.PlayJumpSound();
        SetCrouch(true);
        yield return new WaitForSeconds(0.5f);
        SetCrouch(false);
        SetJump(true);
        rigidBody.AddForce(GetForce());

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
            forceX = ratio * powerWithMultiplier;
            forceY = (1 - ratio) * powerWithMultiplier;
        } else if (angle > 90 && angle <= 180) {
            ratio = (angle - 90) / 90f;
            forceX = (1 - ratio) * powerWithMultiplier;
            forceY = -(ratio) * powerWithMultiplier;
        } else if (angle > 180 && angle <= 270) {
            ratio = (angle - 180) / 90f;
            forceX = -ratio * powerWithMultiplier;
            forceY = -(1 - ratio) * powerWithMultiplier;
        } else if (angle > 270 && angle <= 360) {
            ratio = (angle - 270) / 90f;
            forceX = -(1 - ratio) * powerWithMultiplier;
            forceY = ratio * powerWithMultiplier;
        }
        
        Vector2 force = new Vector2(forceX, forceY);

        return force;
    }

    public void SetPower(float power) {
        power = power > 1 ? 1 : power;
        powerAmount = power * 100;
    }


    void OnCollisionEnter2D(Collision2D collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null) {
            if (rigidBody.velocity.x < 4f && rigidBody.velocity.y < 4f && rigidBody.velocity.x > -4f && rigidBody.velocity.y > -4f && GetJump() == true) {
                SetJump(false);
            }
            platform.AnimateBounce();
            DoPlatformActions(platform);
        }
    }



    private void OnCollisionStay2D(Collision2D collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null) {
            if (rigidBody.velocity.x < 4f && rigidBody.velocity.y < 4f && rigidBody.velocity.x > -4f && rigidBody.velocity.y > -4f && GetJump() == true) { 
                SetJump(false);
            }
        }
    }

    void DoPlatformActions(Platform platform) {
        doingPlatformActions = true;
        StartCoroutine(DoPlatformActionsCoroutine(platform));
    }

    IEnumerator DoPlatformActionsCoroutine(Platform platform) {
       

        int id = platform.id;
        if (id == currentPlatformId) {
            doingPlatformActions = false;
            // exit
            yield break;
        }

        yield return new WaitForSecondsRealtime(.5f);

        bool landed = false;

        if (Helper.CheckRigidBodyContactsHasComponent<Platform>(rigidBody)) {
            landed = true;
        }

        if (landed == true && rigidBody.velocity.x < 0.00001f && rigidBody.velocity.y < 0.00001f && rigidBody.velocity.x > -0.00001f && rigidBody.velocity.y > -0.00001f) {
            currentPlatformId = platform.id;
            if(currentPlatformId > GameController.instance.Score) {
                GameController.instance.NewScore(currentPlatformId);
            }
            if (platform.CompareTag("TransitionPlatform") && PlatformController.instance.transitioning == false) {
                PlatformController.instance.TransitionPlatformAction(platform);
            }
        }

        doingPlatformActions = false;
    }


    public void SetAim(bool value) {
        GetComponent<Animator> ().SetBool (aimHash,value);
    }

    public void SetCrouch(bool value) {
        GetComponent<Animator> ().SetBool (crouchHash,value);
    }

    public bool GetCrouch() {
        return GetComponent<Animator>().GetBool(crouchHash);
    }

    public void SetJump(bool value) {
        GetComponent<Animator> ().SetBool (jumpHash,value);
    }

    public bool GetJump() {
        return GetComponent<Animator>().GetBool(jumpHash);
    }

    public void SetIdle(bool value) {
        GetComponent<Animator> ().SetBool (idleHash,value);
    }

    IEnumerator RespawnCoroutine() {
        Platform currentPlatform = PlatformController.instance.GetPlatformById(0);
        Vector3 pos = currentPlatform.transform.position;
        CameraController.instance.MoveTo(currentPlatform.transform,20f);
        yield return new WaitUntil (() => Camera.main.transform.position.x == pos.x);
        pos.y = GameController.instance.respawnHeight;
        Vector2 velocity = new Vector2 (0, -GameController.instance.fallSpeed);
        rigidBody.velocity = velocity;
        transform.position = pos;
    }

    public void Respawn() {
        StartCoroutine(RespawnCoroutine());
    }

    public Collider2D[] GetRigidbodyContacts(int amount) {
        Collider2D[] colliders = new Collider2D[amount];
        rigidBody.GetContacts(colliders);
        return colliders;
    }


}
