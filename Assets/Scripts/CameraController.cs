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
        Player.Instance.OnPlatformLanded += MoveToPlayerExact;
    }

    public void MoveToTitleScreenPosition() {
        if (GameController.Instance.Playing == false) {
            Vector3 pos = transform.position;
            pos.x = titleScreenPosition;
            transform.position = pos;
        }
    } 

    void MoveToPlayerExact(int platformId) {
        Vector3 end = Player.Instance.transform.position;
        Vector3 pos = transform.position;
        pos.x = end.x;
        pos.y = end.y;
        Move(pos);
    }

    void Move(Vector3 end) {
        startMarker = transform.position;
        endMarker = end;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
        moving = true;
    }

    

    void Update() {
        LerpToNewPosition();
        FollowPlayer();
    }

    void LerpToNewPosition() {
        if (moving == true) {
            float timeDif = Time.time - startTime;
            float t = timeDif / lerpTime;
            float interpolant = ParametricBlend(t);

            transform.position = Vector3.Lerp(startMarker, endMarker, interpolant);


            if (endMarker.x != Player.Instance.transform.position.x || endMarker.y != Player.Instance.transform.position.y) {
                endMarker.x = Player.Instance.transform.position.x;
                endMarker.y = Player.Instance.transform.position.y;
            }

            if (transform.position == endMarker) {
                moving = false;
                finishMoving?.Invoke();
            }
            
        }
    }

    float ParametricBlend(float t) {
        float alpha = 2.1f;
        float sqt = t * t;
        return sqt / (alpha * (sqt - t) + 1.0f);
    }

    void FollowPlayer() {
        if (GetCameraNotMovingAndPlayerIsAlive()) {
            Vector3 playerPos = Player.Instance.transform.position;
            Vector3 pos = transform.position;
            pos.x = playerPos.x;
            transform.position = pos;
        }
    }

    bool GetCameraNotMovingAndPlayerIsAlive() {
        return moving == false && GameController.Instance.Playing == true && Player.Instance.gameObject.activeSelf == true;
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag ("Player")) {
            GameController.Instance.Die ();
        }
    }

    public void MoveTo(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        Move (pos);
    }

    public void MoveToInstant(Transform t) {
        Vector3 pos = transform.position;
        pos.x = t.position.x;
        transform.position = pos;
    }
}
