using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsToggle : Toggle, IChangeableSettingsElement {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } }

    public UnityEvent OnChange { get; } = new UnityEvent();

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

