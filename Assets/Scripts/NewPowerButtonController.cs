using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewPowerButtonController : MonoBehaviour
{
    bool showing = false;

    public CircleGenerator outerRing;
    public CircleGenerator powerRing;

    private void Start() {
        Hide();
    }

    void Update() {
        if (Input.GetButtonDown("Fire1") && !showing) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Objects"));
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && !showing) {
                Show();
            }
        }

        if (Input.GetButton("Fire1") && showing) {
            Vector3 buttonPos = transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetAngle(buttonPos, mousePos);
            SetPower(buttonPos, mousePos);
        }

        if (Input.GetButtonUp("Fire1") && showing) {
            Hide();
            Frog.instance.Jump();
        }
    }

    void SetPower(Vector3 buttonPos, Vector3 mousePos) {
        Vector2 point1 = mousePos;
        Vector2 point2 = buttonPos;

        float distance = Vector2.Distance(point1, point2);

        float size = outerRing.size;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        powerRing.completion = (int)(360 * power);
        powerRing.Generate();

        Frog.instance.SetPower(power);
    }

    void SetAngle(Vector3 buttonPos, Vector3 mousePos) {
        float adjacent = mousePos.x - buttonPos.x;
        float opposite = mousePos.y - buttonPos.y;

        float angle = Mathf.Rad2Deg * Mathf.Atan(adjacent / opposite);

        float modifier = 0;

        if (adjacent >= 0 && opposite >= 0) {
            modifier = 0;
        } else if (adjacent > 0 && opposite < 0) {
            modifier = 180f;
        } else if (adjacent < 0 && opposite < 0) {
            modifier = 180f;
        } else if (adjacent < 0 && opposite > 0) {
            modifier = 360f;
        }

        Frog.instance.SetJumpAngle(angle, modifier);
    }

    void Show() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 position = player.transform.position;
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
