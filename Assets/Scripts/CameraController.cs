using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance = null;

    public Vector3 startMarker;
    public Vector3 endMarker;
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
            Destroy (gameObject);
        }

        DontDestroyOnLoad (gameObject);
    }

    public void Move(Vector3 end) {
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
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            Vector3 pos = GameObject.FindGameObjectWithTag ("StartPlatform").transform.position;
            pos.y += 1f;
            collider.gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
            collider.gameObject.transform.position = pos;
        }
    }

    public void MoveTo(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        Move (pos);
    }
}
