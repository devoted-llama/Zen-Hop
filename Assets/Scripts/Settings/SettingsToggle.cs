using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsToggle : Toggle, IChangeableSettingsElement<bool> {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } }

    public bool Value { get; private set; }

    UnityAction<bool> delegateEvent;

    new void Start() {
        base.Start();
        onValueChanged.AddListener(DoChangeAction);
    }

    public void SetValue(bool value) {
        Value = value;
        SetIsOnWithoutNotify(value);
    }

    public void AddListener(UnityAction<bool> call) {
        delegateEvent += call;
    }

    void DoChangeAction(bool value) {
        Value = value;
        if (delegateEvent != null) {
            delegateEvent(Value);
        }
    }

}

