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
    Vector3 originPos;
    Vector3 currentPos;

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
        

        if (Player.Instance.IsReady() && GetPressStart() && !showing) {
            originPos = GetPressPosition();
            if (GetPressPlayer()) {
                Show(Player.Instance.transform.position);
            }
        }

        if (GetPressHold() && showing) {
            currentPos = GetPressPosition();
            SetAngle();
            SetPower();
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

    bool GetPressPlayer() {
        Vector3 pos = GetPressPosition();
        RaycastHit2D hit = Physics2D.Raycast(pos, -Vector3.forward);
        
        if(hit.collider != null && hit.transform.CompareTag("Player")) {
            return true;
        }
        return false;
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


    void SetPower() {
        Vector2 point1 = currentPos;
        Vector2 point2 = originPos;

        float distance = Vector2.Distance(point1, point2);

        float size = outerRing.Size * 2;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        powerRing.Completion = (int)(360 * power);
        powerRing.Generate();

        Player.Instance.SetPower(power);
    }

    void SetAngle() {
        float adjacent = currentPos.x - originPos.x;
        float opposite = currentPos.y - originPos.y;
        Debug.Log(string.Format("adj {0} op {1}", adjacent, opposite));
        float angle = Mathf.Rad2Deg * Mathf.Atan(adjacent / opposite);

        float modifier = 0;

        if (adjacent >= 0 && opposite >= 0) {
            modifier = 0;
        } else if ((adjacent > 0 && opposite < 0) || (adjacent < 0 && opposite < 0)) {
            modifier = 180f;
        } else if (adjacent < 0 && opposite > 0) {
            modifier = 360f;
        }

        SetLineAngle(angle, modifier);

        Player.Instance.SetJumpAngle(angle, modifier);
    }

    void SetLineAngle(float angle, float modifier) {
        if (!float.IsNaN(angle)) {
            angle = modifier + angle;
            Vector3 eulerAngle = new Vector3(0, 0, angle);
            line.transform.eulerAngles = -eulerAngle;
        }
    }

    void Show(Vector3 position) {
        //Debug.Log(string.Format("Now showing power button at {0}",position));
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
