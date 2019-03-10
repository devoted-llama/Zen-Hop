﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Frog : MonoBehaviour {
    public static Frog instance = null;

    public Text powerAmountText;
    public GameObject aimer;
    public float powerForceMultiplier;
    float powerAmount = 0;
    float powerWithMultiplier { get { return powerAmount * powerForceMultiplier; } }

    int idleHash = Animator.StringToHash ("Idle");
    int crouchHash = Animator.StringToHash ("Crouch");
    int jumpHash = Animator.StringToHash ("Jump");

    public Button powerButton;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        DontDestroyOnLoad (gameObject);
    }



    void Start() {
        SetPower (0);
    }

    public void Jump() {
        SetJump (true);
        GetComponent<Rigidbody2D> ().AddForce (GetForce());
        UpdateAmountText ();
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
        powerAmount = 10 * Mathf.Round (power * 10);
        UpdateAmountText ();
    }

    void UpdateAmountText() {
        powerAmountText.text = (powerAmount).ToString ("F0");
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag ("EndPlatform")) {
            collision.gameObject.tag = "Untagged";
            PlatformController.instance.EndPlatformAction ();
        }

        SetJump (false);
        SetCrouch (false);
        /*if (collision.gameObject.CompareTag ("Platform") || collision.gameObject.CompareTag ("StartPlatform")) {
            CameraController.instance.MoveTo (transform);
        }*/
    }

    public void SetCrouch(bool value) {
        GetComponent<Animator> ().SetBool (crouchHash,value);
    }

    public void SetJump(bool value) {
        GetComponent<Animator> ().SetBool (jumpHash,value);
    }

    public void SetIdle(bool value) {
        GetComponent<Animator> ().SetBool (idleHash,value);
    }


}
