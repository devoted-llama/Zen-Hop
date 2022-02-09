using UnityEngine;


public class CameraController : MonoBehaviour {
    public static CameraController Instance = null;

    Vector3 startMarker;
    Vector3 endMarker;
    float titleScreenPosition = -12f;
    public float TitleScreenPosition { get { return titleScreenPosition; } }
    float startTime;
    float journeyLength;
    float lerpTime = 2f;
    bool moving = false;
    [SerializeField] GameObject followObject;

    public delegate void moveEvent();
    public event moveEvent finishMoving;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        MoveToTitleScreenPosition();   
    }

    private void Start() {
        Player.Instance.OnPlatformLanded += MoveToFollowObjectExact;
    }

    public void MoveToTitleScreenPosition() {
        if (GameController.Instance.Playing == false) {
            Vector3 pos = transform.position;
            pos.x = titleScreenPosition;
            transform.position = pos;
        }
    } 

    void MoveToFollowObjectExact(int platformId) {
        SetEndMarkerToFollowObject();

        startMarker = transform.position;

        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
        moving = true;
    }
    
    void Update() {
        LerpToNewPosition();
        MoveToFollowObject();
    }

    void LerpToNewPosition() {
        if (moving == true) {
            float timeDif = Time.time - startTime;
            float t = timeDif / lerpTime;
            float interpolant = ParametricBlend(t);

            transform.position = Vector3.Lerp(startMarker, endMarker, interpolant);


            ResetEndMarkerIfDifferentToFollowObjectPosition();

            if (transform.position == endMarker) {
                moving = false;
                finishMoving?.Invoke();
            }
            
        }
    }

    void ResetEndMarkerIfDifferentToFollowObjectPosition() {
        if (endMarker.x != followObject.transform.position.x || endMarker.y != followObject.transform.position.y) {
            SetEndMarkerToFollowObject();
        }
    }

    void SetEndMarkerToFollowObject() {
        endMarker = transform.position;
        endMarker.x = followObject.transform.position.x;
        endMarker.y = followObject.transform.position.y;
    }

    float ParametricBlend(float t) {
        float alpha = 2.1f;
        float sqt = t * t;
        return sqt / (alpha * (sqt - t) + 1.0f);
    }

    void MoveToFollowObject() {
        if (GetCameraNotMovingPlayingAndFollowObjectIsActive()) {
            Vector3 followObjectPos = followObject.transform.position;
            Vector3 pos = transform.position;
            pos.x = followObjectPos.x;
            transform.position = pos;
        }
    }

    bool GetCameraNotMovingPlayingAndFollowObjectIsActive() {
        return moving == false && GameController.Instance.Playing == true && followObject.activeSelf == true;
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            GameController.Instance.Die ();
        }
    }

    public void MoveToInstant(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        transform.position = pos;
    }
}
