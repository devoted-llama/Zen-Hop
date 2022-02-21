using UnityEngine.Events;


public interface IChangeableSettingsElement {
    public SettingsData SettingsData { get; }
    public string SettingsKey { get; }
    public void SetSettingsData(SettingsData settingsData);
    public void AddListener(UnityAction<SettingsData> call);
}
