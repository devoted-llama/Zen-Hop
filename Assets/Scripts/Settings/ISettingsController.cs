
using UnityEngine.Events;


public interface ISettingsController {
    public SettingsData Load(string key);
    public void Save(string key, SettingsData preference);
    public UnityEvent<SettingsData> Subscribe(string settingsKey);
}
