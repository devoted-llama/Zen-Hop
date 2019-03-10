using System.Collections;
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
        float div = 90f;

        float ratio = 0f;
        float forceX = 0f;
        float forceY = 0f;

        if (angle >= 0 && angle <= 90) {
            ratio = aimer.transform.localEulerAngles.z / 90f;
            forceX = ratio * powerAmount;
            forceY = (1 - ratio) * powerAmount;
        } else if (angle > 90 && angle <= 180) {
            ratio = (aimer.transform.localEulerAngles.z - 90) / 90f;
            forceX = (1 - ratio)* powerAmount;
            forceY = -(ratio) * powerAmount;
        } else if (angle > 180 && angle <= 270) {
            ratio = (aimer.transform.localEulerAngles.z - 180) / 90f;
            forceX = -ratio * powerAmount;
            forceY = -(1 - ratio) * powerAmount;
        } else if (angle > 270 && angle <= 360) {
            ratio = (aimer.transform.localEulerAngles.z - 270) / 90f;
            forceX = -(1-ratio) * powerAmount;
            forceY = ratio * powerAmount;
        }

        Debug.LogFormat ("x: {0}, y: {1}", forceX, forceY);

        Vector2 force = new Vector2 (forceX, forceY);

        return force;
       
    }

    public void SetPower(float power) {
        powerAmount = power * powerForceMultiplier;
        powerAmount = powerAmount > 1000 ? 1000 : powerAmount;
        UpdateAmountText ();
    }

    void UpdateAmountText() {
        powerAmountText.text = (powerAmount/10f).ToString ("F0");
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag ("EndPlatform")) {
            collision.gameObject.tag = "Untagged";
            PlatformController.instance.EndPlatformAction ();
        }
    }

}
