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

    RigidbodyConstraints2D frogConstraints;
    Rigidbody2D frogRigidBody;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        //DontDestroyOnLoad (gameObject);
    }

    void Start() {
        frogRigidBody = Frog.instance.GetComponent<Rigidbody2D> ();
        frogConstraints = frogRigidBody.constraints;
    }

    public void Move(Vector3 end) {
        startMarker = transform.position;
        endMarker = end;
        startTime = Time.time;
        journeyLength = Vector3.Distance (startMarker, endMarker);
        //frogRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        moving = true;
    }

    void Update() {
        if (moving == true) {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp (startMarker, endMarker, fracJourney);
            if (transform.position == endMarker) {
                moving = false;
                //frogRigidBody.constraints = frogConstraints;
            }
        } else {
            Vector3 pos = transform.position;
            pos.x = Frog.instance.transform.position.x;
            transform.position = pos;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            MoveTo (GameObject.FindGameObjectWithTag ("StartPlatform").transform);
            Frog.instance.RespawnDeath ();
        }
    }

    public void MoveTo(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        Move (pos);
    }
}
