
using UnityEngine.Events;


public interface ISettingsController {
    public bool LoadBool(string key);
    public void Save(string key, bool value);
    public UnityEvent<bool> SubscribeToBool(string settingsKey);
}
