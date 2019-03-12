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

    int idleHash = Animator.StringToHash ("Idle");
    int aimHash = Animator.StringToHash ("Aim");
    int crouchHash = Animator.StringToHash ("Crouch");
    int jumpHash = Animator.StringToHash ("Jump");

    public Button powerButton;

    //public RectTransform powerBarOuter;
    //public RectTransform powerBarInner;

    public SpriteMask powerStickMask;
    float powerStickMaskSize = 2.53f;

    Vector3 powerBarRight;

    public Rigidbody2D rigidBody;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        //DontDestroyOnLoad (gameObject);
    }



    void Start() {
        rigidBody = GetComponent<Rigidbody2D> ();
        SetPower (0);
        RespawnNoDeath ();
    }

    public void Jump() {
        if (rigidBody.velocity.x == 0 && rigidBody.velocity.y == 0) {
            StartCoroutine (JumpCoroutine ());
        }
    }

    IEnumerator JumpCoroutine() {
        SetCrouch (true);
        yield return new  WaitForSeconds (0.5f);
        SetCrouch (false);
        SetJump (true);
        rigidBody.AddForce (GetForce ());
        SetPower (0);
        UpdateAmountUI ();
    }

    public void SetAimerAngle(float angle, float modifier = 0) {
        if (!float.IsNaN (angle)) {
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
            forceX = (1 - ratio)* powerWithMultiplier;
            forceY = -(ratio) * powerWithMultiplier;
        } else if (angle > 180 && angle <= 270) {
            ratio = (aimer.transform.localEulerAngles.z - 180) / 90f;
            forceX = -ratio * powerWithMultiplier;
            forceY = -(1 - ratio) * powerWithMultiplier;
        } else if (angle > 270 && angle <= 360) {
            ratio = (aimer.transform.localEulerAngles.z - 270) / 90f;
            forceX = -(1-ratio) * powerWithMultiplier;
            forceY = ratio * powerWithMultiplier;
        }

        //Debug.LogFormat ("x: {0}, y: {1}", forceX, forceY);

        Vector2 force = new Vector2 (forceX, forceY);

        return force;
       
    }

    public void SetPower(float power) {
        power = power > 1 ? 1 : power;
        powerAmount = power * 100;
        UpdateAmountUI ();
    }

    void UpdateAmountUI() {
        //powerAmountText.text = (powerAmount).ToString ("F0");
        //powerBarInner.rectTransform.
        //float size = powerBarOuter.sizeDelta.x * (powerAmount / 100f);
        //powerBarInner.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,0,size);

        float size = (powerStickMaskSize - 1) * (powerAmount / 100f);
        Vector3 pos = powerStickMask.transform.localPosition;
        pos.y = 1 + size;
        powerStickMask.transform.localPosition = pos;
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag ("EndPlatform")) {
            PlatformController.instance.EndPlatformAction ();
        }
        //SetCrouch (false);
        /*if (collision.gameObject.CompareTag ("Platform") || collision.gameObject.CompareTag ("StartPlatform")) {
            CameraController.instance.MoveTo (transform);
        }*/
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if((collision.gameObject.CompareTag("Platform") || 
            collision.gameObject.CompareTag("StartPlatform") || 
            collision.gameObject.CompareTag("EndPlatform")) && 
            GetJump() == true) {
            SetJump (false);
        }
    }

    public void SetAim(bool value) {
        GetComponent<Animator> ().SetBool (aimHash,value);
    }

    public void SetCrouch(bool value) {
        GetComponent<Animator> ().SetBool (crouchHash,value);
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

    void Respawn() {
        StartCoroutine (RespawnCoroutine ());
    }

    IEnumerator RespawnCoroutine() {
        yield return new WaitForSecondsRealtime(GameController.instance.respawnTime);
        Vector3 pos = GameObject.FindGameObjectWithTag ("StartPlatform").transform.position;
        pos.y = GameController.instance.respawnHeight;
        Vector2 velocity = new Vector2 (0, -GameController.instance.fallSpeed);
        rigidBody.velocity = velocity;
        transform.position = pos;
    }

    public void RespawnNoDeath() {
        Respawn ();
    }

    public void RespawnDeath() {
        GameController.instance.Die ();
        if (GameController.instance.lives > 0) {
            Respawn ();
        }
    }


}
