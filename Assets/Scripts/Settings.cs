﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
public class SettingsEvent : UnityEvent <bool> { }
public class Settings : MonoBehaviour {
    public static Settings Instance { get; private set; } = null;

    static IDictionary<string, SettingsEvent> events = new Dictionary<string, SettingsEvent>();

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
            events.Add(item.key, ev);
        }
    }

    public static bool Load(string key) {
        if(Instance.settingsKeys.settingKeyValues.Find(item => item.key == key) is SettingKeyValue s) {
            return PlayerPrefs.GetInt(key, s.value ? 1 : 0) == 0 ? false : true;
        } else {
            throw new UnityException("You're trying to load a key which doesn't exist.");
        }
    }

    public static void Save(string key, bool preference) {
        PlayerPrefs.SetInt(key, preference ? 1 : 0);
        PlayerPrefs.Save();
        SettingsEvent e;
        if (events.TryGetValue(key, out e)) {
            e.Invoke(preference);
        }
    }

    public static SettingsEvent Subscribe(string key) {
        SettingsEvent ev;
        if (events.TryGetValue(key, out ev)) {
            return ev;
        }
        throw new UnityException($"No such event with key '{key}'");
    }

}