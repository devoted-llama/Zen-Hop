using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsToggle : Toggle, IChangeableSettingsElement {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }

    UnityEvent _onChange = new UnityEvent();
    public UnityEvent OnChange { get { return _onChange; } private set { _onChange = value; } }

    public dynamic Value { get { return isOn; } }

    new void Start() {
        base.Start();
        onValueChanged.AddListener(delegate {
            OnChange.Invoke();
        });
    }

    public void SetValue(dynamic value) {
        SetIsOnWithoutNotify(value);
    }
}

