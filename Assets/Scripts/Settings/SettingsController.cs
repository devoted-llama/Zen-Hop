using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class SettingsEvent : UnityEvent <dynamic> { }
public class SettingsController : MonoBehaviour {
    public static SettingsController Instance { get; private set; } = null;

    static IDictionary<string, SettingsEvent> s_events = new Dictionary<string, SettingsEvent>();

    [SerializeField] SettingsKeys settingsKeys;

    void Awake() {
        InitialiseSingleton();
        SetupEvents();
    }

    void InitialiseSingleton() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }
    
    void SetupEvents() {
        foreach (var item in settingsKeys.settingKeyValues) {
            SettingsEvent ev = new SettingsEvent();
            s_events.Add(item.key, ev);
        }
    }

    public bool Load(string key) {
        if(Instance.settingsKeys.settingKeyValues.Find(item => item.key == key) is SettingKeyValue s) {
            
            return PlayerPrefs.GetInt(key, s.value ? 1 : 0) == 0 ? false : true;
            
        } else {
            throw new UnityException("You're trying to load a key which doesn't exist.");
        }
    }

    public void Save(string key, bool preference) {
        PlayerPrefs.SetInt(key, preference ? 1 : 0);
        
        PlayerPrefs.Save();
        SettingsEvent e;
        if (s_events.TryGetValue(key, out e)) {
            e.Invoke(preference);
        }
    }

    public SettingsEvent Subscribe(string key) {
        SettingsEvent ev;
        if (s_events.TryGetValue(key, out ev)) {
            return ev;
        }
        throw new UnityException($"No such event with key '{key}'");
    }

}
