using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerButtonController : MonoBehaviour
{
    bool showing = false;
    bool handheld = false;
    Touch touch;

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
        if (handheld && Input.touchCount > 0) {
            touch = Input.GetTouch(0);
        }
        DoAction();
    }

    void SetIsHandHeld() {
        handheld = SystemInfo.deviceType == DeviceType.Handheld;
        Debug.Log(string.Format("handheld set to {0}", handheld));
    }

    void DoAction() {
        Vector3 pressPos = GetPressPosition();

        if (Player.Instance.IsReady() && GetPressStart() && !showing) {
            Show(pressPos);
        }

        if (GetPressHold() && showing) {
            Vector3 buttonPos = transform.position;
            SetAngle(buttonPos, pressPos);
            SetPower(buttonPos, pressPos);
        }

        if (GetPressEnd() && showing) {
            Hide();
            Player.Instance.Jump();
        }
    }

    Vector3 GetPressPosition() {
        if (handheld) {
            return Camera.main.ScreenToWorldPoint(touch.position);
        }
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    bool GetPressStart() {
        if(handheld) {
            return touch.phase == TouchPhase.Began;
        }
        return Input.GetButtonDown("Fire1");
    }

    bool GetPressHold() {
        if (handheld) {
            return touch.phase == TouchPhase.Moved;
        }
        return Input.GetButton("Fire1");
    }

    bool GetPressEnd() {
        if (handheld) {
            return touch.phase == TouchPhase.Ended;
        }
        return Input.GetButtonUp("Fire1");
    }


    void SetPower(Vector3 buttonPos, Vector3 cursorPos) {
        Vector2 point1 = cursorPos;
        Vector2 point2 = buttonPos;

        float distance = Vector2.Distance(point1, point2);

        float size = outerRing.Size * 2;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        powerRing.Completion = (int)(360 * power);
        powerRing.Generate();

        Player.Instance.SetPower(power);
    }

    void SetAngle(Vector3 buttonPos, Vector3 cursorPos) {
        float adjacent = cursorPos.x - buttonPos.x;
        float opposite = cursorPos.y - buttonPos.y;

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
        Debug.Log(string.Format("Now showing power button at {0}",position));
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
