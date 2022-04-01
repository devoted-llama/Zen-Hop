using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using ProudLlama.CircleGenerator;

public class PowerButtonController : MonoBehaviour, ISettable<bool> {
    public static PowerButtonController Instance { get; private set; } = null;
    bool _showing = false;
    bool _handheld = false;
    Touch _touch;
    Vector3 _originPos;
    Vector3 _currentPos;

    [SerializeField] DashCircleGenerator _outerRing;
    [SerializeField] StrokeCircleGenerator _powerRing;
    [SerializeField] LineRenderer _line;
    [SerializeField] float _powerAreaMultiplier = 2.7f;

    GraphicRaycaster _raycaster;
    EventSystem _eventSystem;

    [SerializeField] string _settingsKey = "playerPressAnywhere";
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }

    bool _playerPressAnywhere;

    void Awake() {
        InitialiseSingleton();
    }

    void InitialiseSingleton() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        _raycaster = FindObjectOfType<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();

        SetIsHandHeld();
        Hide();
    }

    void Update() {
        if (_handheld && Input.touchCount > 0) {
            _touch = Input.GetTouch(0);
        }
        DoAction();
    }

    void SetIsHandHeld() {
        _handheld = SystemInfo.deviceType == DeviceType.Handheld;
    }

    void DoAction() {
        if (Player.Instance.IsReady() && GetPressStart() && !_showing) {
            _originPos = GetPressPosition();
            if (GetPressPlayer()) {
                SetAngleZero();
                SetPowerZero();
                Show(Player.Instance.transform.position);
            }
        }

        if (GetPressHold() && _showing) {
            _currentPos = GetPressPosition();
            SetAngle();
            SetPower();
        }

        if (GetPressEnd() && _showing) {
            Hide();
            Player.Instance.Jump();
        }
    }

    bool GetPressPlayer() {
        if (GetTouchedUI()) {
            return false;
        }
        if (!_playerPressAnywhere) {
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
        PointerEventData pointerEventData = new PointerEventData(_eventSystem);
        pointerEventData.position = GetPressPositionRaw();
        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results) {
            return true;
        }
        return false;
    }

    Vector3 GetPressPositionRaw() {
        if (_handheld) {
            return _touch.position;
        }
        return Input.mousePosition;
    }

    Vector3 GetPressPosition() {
        if (_handheld) {
            return Camera.main.ScreenToWorldPoint(_touch.position);
        }
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    bool GetPressStart() {
        if (_handheld) {
            return _touch.phase == TouchPhase.Began;
        }
        return Input.GetButtonDown("Fire1");


    }

    bool GetPressHold() {
        if (_handheld) {
            return _touch.phase == TouchPhase.Moved;
        }
        return Input.GetButton("Fire1");
    }

    bool GetPressEnd() {
        if (_handheld) {
            return _touch.phase == TouchPhase.Ended;
        }
        return Input.GetButtonUp("Fire1");
    }


    void SetPower() {
        Vector2 point1 = _currentPos;
        Vector2 point2 = _originPos;

        float distance = Vector2.Distance(point1, point2);

        float size = _outerRing.CircleData.Radius * _powerAreaMultiplier;

        float power = distance / size;
        power = power > 1 ? 1 : power;

        CircleData cd = _powerRing.CircleData;
        cd.Completion = (int)(360 * power);
        _powerRing.CircleData = cd;
        _powerRing.Generate();

        /* Don't reference player!! */
        Player.Instance.SetPower(power);
    }

    void SetPowerZero() {
        CircleData cd = _powerRing.CircleData;
        cd.Completion = 0;
        _powerRing.CircleData = cd;
        _powerRing.Generate();
        /* Don't reference Player!! */
        Player.Instance.SetPower(0);
    }

    void SetAngle() {
        float adjacent = _currentPos.x - _originPos.x;
        float opposite = _currentPos.y - _originPos.y;
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
        /* Don't reference player!! */
        Player.Instance.SetJumpAngle(0, 0);
    }

    void SetLineAngle(float angle, float modifier) {
        if (!float.IsNaN(angle)) {
            angle = modifier + angle;
            Vector3 eulerAngle = new Vector3(0, 0, angle);
            _line.transform.eulerAngles = -eulerAngle;
        }
    }

    void Show(Vector3 position) {
        position.z = transform.position.z;
        transform.position = position;

        Vector3 scale = transform.localScale;
        scale.x = 1;
        scale.y = 1;
        transform.localScale = scale;
        _showing = true;
    }

    void Hide() {
        Vector3 scale = transform.localScale;
        scale.x = 0;
        scale.y = 0;
        transform.localScale = scale;
        _showing = false;
    }

    public void RegisterSettings(bool value) {
        _playerPressAnywhere = value;
    }
}
