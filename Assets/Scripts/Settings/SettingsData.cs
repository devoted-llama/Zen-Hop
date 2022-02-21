using UnityEngine;
[System.Serializable]
public struct SettingsData {
    [SerializeField]bool _bool;
    public bool Bool { get { return _bool;  } private set { _bool = value; } }

    public void Set(bool value) {
        Bool = value;
    }
}
