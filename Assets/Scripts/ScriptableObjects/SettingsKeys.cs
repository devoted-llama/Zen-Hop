using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SettingsKeys", menuName = "ScriptableObjects/SettingsKeysScriptableObject", order = 1)]
public class SettingsKeys : ScriptableObject
{
    public List<SettingKeyValue> settingKeyValues;
}

[System.Serializable]
public class SettingKeyValue {
    public string key;
    public bool value;
} 
