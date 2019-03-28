using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance = null;

    public Vector3 startMarker;
    public Vector3 endMarker;
    public const float titleScreenPosition = -12f;
    float speed = 10.0f;
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
            pos.x = titleScreenPosition;
            transform.position = pos;
        }
    }

    public void Move(Vector3 end, float speed = 10f) {
        this.speed = speed;
        startMarker = transform.position;
        endMarker = end;
        startTime = Time.time;
        journeyLength = Vector3.Distance (startMarker, endMarker);
        moving = true;
    }

    void Update() {
        if (moving == true) {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp (startMarker, endMarker, fracJourney);
            if (transform.position == endMarker) {
                moving = false;
                if(finishMoving != null) {
                    finishMoving();
                }
            }
        } else if(GameController.instance.playing == true){
            Vector3 pos = transform.position;
            pos.x = Frog.instance.transform.position.x;
            transform.position = pos;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            GameController.instance.Die ();
        }
    }

    public void MoveTo(Transform t, float speed = 10f) {
        this.speed = speed;
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        Move (pos);
    }
}
