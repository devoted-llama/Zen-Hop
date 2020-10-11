using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance = null;

    Vector3 startMarker;
    Vector3 endMarker;
    [SerializeField]
    float titleScreenPosition = -12f;
    public float TitleScreenPosition { get { return titleScreenPosition; } }
    [SerializeField]
    float speed = 10.0f;
    public float Speed { get { return speed; } }
    float startTime;
    float journeyLength;

    bool moving = false;

    public delegate void moveEvent();
    public event moveEvent finishMoving;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        MoveToTitleScreenPosition();
        
    }
    public void MoveToTitleScreenPosition() {
        if (GameController.instance.playing == false) {
            Vector3 pos = transform.position;
            pos.x = TitleScreenPosition;
            transform.position = pos;
        }
    }

    public void Move(Vector3 end) {
        startMarker = transform.position;
        endMarker = end;
        startTime = Time.time;
        journeyLength = Vector3.Distance (startMarker, endMarker);
        moving = true;
    }

    void LerpToNewPosition() {
        if (moving == true) {
            float distCovered = (Time.time - startTime) * Speed;
            float fractionJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker, endMarker, fractionJourney);
            if (transform.position == endMarker) {
                moving = false;
                finishMoving?.Invoke();
            }
        }
    }

    bool GetCameraNotMovingAndPlayerIsAlive() {
        return moving == false && GameController.instance.playing == true && Player.Instance.gameObject.activeSelf == true;
    }

    void FollowPlayer() {
        if (GetCameraNotMovingAndPlayerIsAlive()) {
            Vector3 pos = transform.position;
            pos.x = Player.Instance.transform.position.x;
            transform.position = pos;
        }
    }

    void Update() {
        LerpToNewPosition();
        FollowPlayer();
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            GameController.instance.Die ();
        }
    }

    public void MoveTo(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        Move (pos);
    }
}
