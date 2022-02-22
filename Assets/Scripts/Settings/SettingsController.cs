using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class SettingsController : MonoBehaviour, ISettingsController {
    IDictionary<string, UnityEvent<bool>> events = new Dictionary<string, UnityEvent<bool>>();

    [SerializeField] SettingsKeys settingsKeys;

    void Awake() {
        SetupEvents();
    }

    
    void SetupEvents() {
        foreach (var item in settingsKeys.settingKeyValues) {
            UnityEvent<bool> ev = new UnityEvent<bool>();
            events.Add(item.key, ev);
        }
    }


    public bool LoadBool(string key) {
        if (settingsKeys.settingKeyValues.Find(item => item.key == key) is SettingKeyValue s) {
            return PlayerPrefs.GetInt(key, s.value ? 1 : 0) == 0 ? false : true;
        } else {
            throw new UnityException("You're trying to load a key which doesn't exist.");
        }
    }

    public void Save(string key, bool value) {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        
        PlayerPrefs.Save();
        UnityEvent<bool> e;
        if (events.TryGetValue(key, out e)) {
            e.Invoke(value);
        }
    }

    public UnityEvent<bool> SubscribeToBool(string key) {
        UnityEvent<bool> ev;
        if (events.TryGetValue(key, out ev)) {
            return ev;
        }
        throw new UnityException($"No such event with key '{key}'");
    }

}
