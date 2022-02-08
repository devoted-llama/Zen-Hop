using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PowerButtonController : MonoBehaviour {
    public static PowerButtonController Instance { get; private set; } = null;
    bool showing = false;
    bool handheld = false;
    Touch touch;
    Vector3 originPos;
    Vector3 currentPos;

    [SerializeField] CircleGenerator outerRing;
    [SerializeField] CircleGenerator powerRing;
    [SerializeField] LineRenderer line;
    [SerializeField] float powerAreaMultiplier = 2f;

    GraphicRaycaster raycaster;
    EventSystem eventSystem;

    /* True for press anywhere, false for press player/ball */
    public bool PlayerPressPreference { get; private set; } = true;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        LoadPlayerPressPreference();
    }

    private void Start() {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

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
    }

    void DoAction() {
        if (Player.Instance.IsReady() && GetPressStart() && !showing) {
            originPos = GetPressPosition();
            if (GetPressPlayer()) {
                SetAngleZero();
                SetPowerZero();
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

    bool GetPressPlayer() {
        if(GetTouchedUI()) {
            return false;
        }
        if (!PlayerPressPreference) {
            Vector3 pos = GetPressPosition();
            RaycastHit2D hit = Physics2D.Raycast(pos, -Vector3.forward);
            if (hit.collider != null && hit.transform.CompareTag("Player")) {
                return true;
            }
            return false;
        } 
        return true;
    }

    bool GetTouchedUI() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = GetPressPositionRaw();
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results) {
            return true;
        }
        return false;
    }

    Vector3 GetPressPositionRaw() {
        if (handheld) {
            return touch.position;
        }
        return Input.mousePosition;
    }

    Vector3 GetPressPosition() {
        if (handheld) {
            return Camera.main.ScreenToWorldPoint(touch.position);
        }
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    bool GetPressStart() {
        if (handheld) {
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


    void SetPower() {
        Vector2 point1 = currentPos;
        Vector2 point2 = originPos;

        float distance = Vector2.Distance(point1, point2);

        float size = outerRing.Size * powerAreaMultiplier;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        powerRing.Completion = (int)(360 * power);
        powerRing.Generate();

        Player.Instance.SetPower(power);
    }

    void SetPowerZero() {
        powerRing.Completion = 0;
        powerRing.Generate();
        Player.Instance.SetPower(0);
    }

    void SetAngle() {
        float adjacent = currentPos.x - originPos.x;
        float opposite = currentPos.y - originPos.y;
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

    void SetAngleZero() {
        SetLineAngle(0, 0);
        Player.Instance.SetJumpAngle(0, 0);
    }

    void SetLineAngle(float angle, float modifier) {
        if (!float.IsNaN(angle)) {
            angle = modifier + angle;
            Vector3 eulerAngle = new Vector3(0, 0, angle);
            line.transform.eulerAngles = -eulerAngle;
        }
    }

    void Show(Vector3 position) {
        position.z = transform.position.z;
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

    public void ChangePlayerPressStateAndSetPreference() {
        if (PlayerPressPreference != true) {
            PlayerPressPreference = true;
            SetPlayerPressPreference(true);
        } else {
            PlayerPressPreference = false;
            SetPlayerPressPreference(false);
        }
    }

    void LoadPlayerPressPreference() {
        if (PlayerPrefs.HasKey("playerPressPreference")) {
            PlayerPressPreference = PlayerPrefs.GetInt("playerPressPreference") == 0 ? false : true;
        }
    }

    void SetPlayerPressPreference(bool preference) {
        PlayerPrefs.SetInt("playerPressPreference", preference == false ? 0 : 1);
        PlayerPrefs.Save();
    }


}
