using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Frog : MonoBehaviour {
    public static Frog instance = null;

    public GameObject aimer;
    public float powerForceMultiplier;
    float powerAmount = 0;
    float powerWithMultiplier { get { return powerAmount * powerForceMultiplier; } }

    int idleHash = Animator.StringToHash("Idle");
    int aimHash = Animator.StringToHash("Aim");
    int crouchHash = Animator.StringToHash("Crouch");
    int jumpHash = Animator.StringToHash("Jump");

    public Button powerButton;

    public SpriteMask powerStickMask;
    float powerStickMaskSize = 2.53f;

    public Rigidbody2D rigidBody;

    public int currentPlatformId = 0;

    public bool doingPlatformActions = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        GameStartSetActive();
    }

    void GameStartSetActive() {
        if(GameController.instance.playing == false) {
            gameObject.SetActive(false);
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
        UpdateAmountUI();
    }

    public void SetAimerAngle(float angle, float modifier = 0) {
        if (!float.IsNaN(angle)) {
            Quaternion qAngle = aimer.transform.localRotation;
            Vector3 eulerAngle = qAngle.eulerAngles;
            eulerAngle.z = modifier + angle;
            qAngle.eulerAngles = eulerAngle;
            aimer.transform.localRotation = qAngle;
        }
    }


    Vector2 GetForce() {
        float angle = aimer.transform.localEulerAngles.z;

        float ratio = 0f;
        float forceX = 0f;
        float forceY = 0f;

        if (angle >= 0 && angle <= 90) {
            ratio = aimer.transform.localEulerAngles.z / 90f;
            forceX = ratio * powerWithMultiplier;
            forceY = (1 - ratio) * powerWithMultiplier;
        } else if (angle > 90 && angle <= 180) {
            ratio = (aimer.transform.localEulerAngles.z - 90) / 90f;
            forceX = (1 - ratio) * powerWithMultiplier;
            forceY = -(ratio) * powerWithMultiplier;
        } else if (angle > 180 && angle <= 270) {
            ratio = (aimer.transform.localEulerAngles.z - 180) / 90f;
            forceX = -ratio * powerWithMultiplier;
            forceY = -(1 - ratio) * powerWithMultiplier;
        } else if (angle > 270 && angle <= 360) {
            ratio = (aimer.transform.localEulerAngles.z - 270) / 90f;
            forceX = -(1 - ratio) * powerWithMultiplier;
            forceY = ratio * powerWithMultiplier;
        }

        //Debug.LogFormat ("x: {0}, y: {1}", forceX, forceY);

        Vector2 force = new Vector2(forceX, forceY);

        return force;

    }

    public void SetPower(float power) {
        power = power > 1 ? 1 : power;
        powerAmount = power * 100;
        UpdateAmountUI();
    }

    void UpdateAmountUI() {

        float size = (powerStickMaskSize - 1) * (powerAmount / 100f);
        Vector3 pos = powerStickMask.transform.localPosition;
        pos.y = 1 + size;
        powerStickMask.transform.localPosition = pos;
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Platform") ||
            collision.gameObject.CompareTag("StartPlatform") ||
            collision.gameObject.CompareTag("TransitionPlatform")) {
            Platform platform = collision.gameObject.GetComponent<Platform>();
            platform.AnimateBounce();
            int id = platform.id;
            if (id != Frog.instance.currentPlatformId) {
                
                DoPlatformActions(platform);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform != null && GetJump() == true) {
            SetJump(false);
        }
    }

    void DoPlatformActions(Platform platform) {
        doingPlatformActions = true;
        StartCoroutine(DoPlatformActionsCoroutine(platform));
    }

    IEnumerator DoPlatformActionsCoroutine(Platform platform) {
        //CameraController.instance.MoveTo(transform, 5f);
        yield return new WaitForSecondsRealtime(.5f);

        bool landed = false;
        int colliderSize = 2;
        Collider2D[] colliders = new Collider2D[colliderSize];
        rigidBody.GetContacts(colliders);
        for (int i = 0; i < colliderSize; i++) {

            if (colliders[i] != null) { 
                platform = colliders[i].GetComponent<Platform>();
                if (platform != null) {
                    landed = true;
                }
            }
        }

        if (landed == true && rigidBody.velocity.x < 0.00001f && rigidBody.velocity.y < 0.00001f) {
            currentPlatformId = platform.id;
            if(currentPlatformId > GameController.instance.Score) {
                GameController.instance.NewScore(currentPlatformId);
            }
            if (platform.CompareTag("TransitionPlatform") && PlatformController.instance.transitioning == false) {
                PlatformController.instance.TransitionPlatformAction(platform);
            }
            //CameraController.instance.MoveTo(transform, 5f);
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


}
