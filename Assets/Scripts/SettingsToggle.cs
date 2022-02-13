using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : Toggle, ISettingsKey {
    [SerializeField] string _settingsKey;
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }
}

