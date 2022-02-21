using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettingsToggle : Toggle, IChangeableSettingsElement {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } }

    public SettingsData SettingsData { get; private set; }

    UnityAction<SettingsData> delegateEvent;

    new void Start() {
        base.Start();
        onValueChanged.AddListener(OnChange);
    }

    public void SetSettingsData(SettingsData settingsData) {

        
        Set(settingsData);
    }

    void Set(SettingsData settingsData) {
        SettingsData = settingsData;
        SetIsOnWithoutNotify(SettingsData.Bool);
    }

    public void AddListener(UnityAction<SettingsData> call) {
        delegateEvent += call;
        onValueChanged.AddListener(OnChange);
    }

    void OnChange(bool value) {
        SettingsData sd = SettingsData;
        sd.Set(value);
        SettingsData = sd;
        delegateEvent(SettingsData);
    }

}

