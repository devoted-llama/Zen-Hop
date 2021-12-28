using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerButtonController : MonoBehaviour
{
    bool showing = false;
    bool handheld = false;

    [SerializeField]
    CircleGenerator outerRing;
    [SerializeField]
    CircleGenerator powerRing;
    [SerializeField]
    LineRenderer line;

    private void Start() {
        SetIsHandHeld();
        Hide();
    }

    void Update() {
        
        if (handheld) {
            DoHandHeldAction();
        } else {
            DoDesktopAction();
        }
    }

    void SetIsHandHeld() {
        handheld = SystemInfo.deviceType == DeviceType.Handheld;
    }
       
    void DoHandHeldAction() {
        if (Input.touchCount > 0) {

            Touch touch = Input.GetTouch(0);

            if (Player.Instance.IsReady() && touch.phase == TouchPhase.Began && !showing) {
                Show(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            if (touch.phase == TouchPhase.Moved && showing) {
                Vector3 buttonPos = transform.position;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetAngle(buttonPos, mousePos);
                SetPower(buttonPos, mousePos);
            }

            if (touch.phase == TouchPhase.Ended && showing) {

                Hide();
                Player.Instance.Jump();
            }
        }
    }

    void DoDesktopAction() {
        if (Player.Instance.IsReady() && Input.GetButtonDown("Fire1") && !showing) {
            Show(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetButton("Fire1") && showing) {
            Vector3 buttonPos = transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetAngle(buttonPos, mousePos);
            SetPower(buttonPos, mousePos);
        }

        if (Input.GetButtonUp("Fire1") && showing) {
            Hide();
            Player.Instance.Jump();
        }
    }


    void SetPower(Vector3 buttonPos, Vector3 mousePos) {
        Vector2 point1 = mousePos;
        Vector2 point2 = buttonPos;

        float distance = Vector2.Distance(point1, point2);

        float size = outerRing.Size;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        powerRing.Completion = (int)(360 * power);
        powerRing.Generate();

        Player.Instance.SetPower(power);
    }

    void SetAngle(Vector3 buttonPos, Vector3 mousePos) {
        float adjacent = mousePos.x - buttonPos.x;
        float opposite = mousePos.y - buttonPos.y;

        float angle = Mathf.Rad2Deg * Mathf.Atan(adjacent / opposite);

        float modifier = 0;

        if (adjacent >= 0 && opposite >= 0) {
            modifier = 0;
        } else if ((adjacent > 0 && opposite < 0) || (adjacent < 0 && opposite < 0)) {
            modifier = 180f;
        } else if (adjacent < 0 && opposite > 0) {
            modifier = 360f;
        }

        SetLineAngle(angle);

        Player.Instance.SetJumpAngle(angle, modifier);
    }

    void SetLineAngle(float angle) {
        if (!float.IsNaN(angle)) {
            Vector3 eulerAngle = new Vector3(0, 0, angle);
            line.transform.eulerAngles = -eulerAngle;
        }
    }

    void Show(Vector3 position) {
        Player player = Player.Instance;
        position.z = player.transform.position.z;
        transform.position = position;

        Vector3 scale = transform.localScale;
        scale.x = 1;
        scale.y = 1;
        transform.localScale = scale;
        showing = true;
    }

    void Hide() {
        Vector3 scale = transform.localScale;
        scale.x = 0;
        scale.y = 0;
        transform.localScale = scale;
        showing = false;
    }


}
