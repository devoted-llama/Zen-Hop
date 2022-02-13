using UnityEngine;

public abstract class SettingsListener : MonoBehaviour, ISettingsKey {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }
    protected bool SettingsState;


    protected void Start() {
        SettingsEvent e = Settings.Subscribe(SettingsKey);
        e.AddListener(SetSettingState);
        GetInitialSettingsState();
    }

    void SetSettingState(bool isOn) {
        SettingsState = isOn;
        RegisterSettings();
    }

    void GetInitialSettingsState() {
        SettingsState = Settings.Load(SettingsKey);
        RegisterSettings();
    }

    protected abstract void RegisterSettings();
}
