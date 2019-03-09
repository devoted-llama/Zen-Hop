using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Frog : MonoBehaviour {

    public Text powerAmountText;
    public GameObject aimer;
    public float powerForceMultiplier;
    float powerAmount = 0;

    public Slider powerSlider;

    void Start() {
        SetPower ();
    }

    void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetButtonUp ("Fire1")) {
                GetComponent<Rigidbody2D> ().AddForce (GetForce());
                //powerAmount = 0;
                UpdateAmountText ();
            }


            if (Input.GetButton ("Fire1")) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

                float adjacent = mousePos.x - transform.position.x;
                float opposite = mousePos.y - transform.position.y;

                adjacent = adjacent < 0 ? 0 : adjacent;
                opposite = opposite < 0 ? 0 : opposite;

                //Debug.LogFormat ("{0},{1}", adjacent, opposite);

                float angle = Mathf.Rad2Deg * Mathf.Atan (opposite / adjacent);

                float hypotenuse = opposite / Mathf.Sin (Mathf.Deg2Rad*angle);

                //Debug.LogFormat ("h: {0}, Dist: {1}, sin: {2}, angle: {3}", hypotenuse, Vector3.Distance(mousePos,transform.position),Mathf.Sin (Mathf.Deg2Rad*angle), angle);
            
                if (!float.IsNaN (angle)) {
                    Quaternion qAngle = aimer.transform.localRotation;
                    Vector3 eulerAngle = qAngle.eulerAngles;
                    eulerAngle.z = 90 - angle;
                    qAngle.eulerAngles = eulerAngle;
                    aimer.transform.localRotation = qAngle;

                    /*if (!float.IsNaN(hypotenuse)) {
                        powerAmount = hypotenuse * powerForceMultiplier;
                        powerAmount = powerAmount > 100 ? 100 : powerAmount;
                        UpdateAmountText ();
                    }*/
                }
            }
        }
    }

    Vector2 GetForce() {
        float ratio = aimer.transform.localEulerAngles.z / 90f;

        Vector2 force = new Vector2 (ratio*powerAmount*powerForceMultiplier, (1 - ratio)*powerAmount*powerForceMultiplier);

        return force;
       
    }

    public void SetPower() {
        powerAmount = powerSlider.value * powerSlider.maxValue;
        UpdateAmountText ();
    }

    void UpdateAmountText() {
        powerAmountText.text = powerAmount.ToString ("F0") + "%";
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag ("EndPlatform")) {
            collision.gameObject.tag = "Untagged";
            PlatformController.instance.EndPlatformAction ();
        }
    }

}
