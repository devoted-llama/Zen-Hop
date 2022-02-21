using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class SettingsController : MonoBehaviour, ISettingsController {
    IDictionary<string, UnityEvent<SettingsData>> events = new Dictionary<string, UnityEvent<SettingsData>>();

    [SerializeField] SettingsKeys settingsKeys;

    void Awake() {
        SetupBoolEvents();
    }

    
    void SetupBoolEvents() {
        foreach (var item in settingsKeys.settingKeyValues) {
            UnityEvent<SettingsData> ev = new UnityEvent<SettingsData>();
            events.Add(item.key, ev);
        }
    }

    public SettingsData Load(string key) {
        if(settingsKeys.settingKeyValues.Find(item => item.key == key) is SettingKeyValue s) {
            SettingsData sd = new SettingsData();
            bool value = PlayerPrefs.GetInt(key, s.data.Bool ? 1 : 0) == 0 ? false : true;
            sd.Set(value);
            return sd;
            
        } else {
            throw new UnityException("You're trying to load a key which doesn't exist.");
        }
    }

    public void Save(string key, SettingsData data) {
        PlayerPrefs.SetInt(key, data.Bool ? 1 : 0);
        
        PlayerPrefs.Save();
        UnityEvent<SettingsData> e;
        if (events.TryGetValue(key, out e)) {
            e.Invoke(data);
        }
    }

    public UnityEvent<SettingsData> Subscribe(string key) {
        UnityEvent<SettingsData> ev;
        if (events.TryGetValue(key, out ev)) {
            return ev;
        }
        throw new UnityException($"No such event with key '{key}'");
    }

}
