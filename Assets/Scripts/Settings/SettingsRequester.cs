using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SettingsRequester : MonoBehaviour {
    public static SettingsRequester Instance { get; private set; } = null;
    [SerializeField, RestrictToType(typeof(ISettingsController))] Object _iSettingsControllerObj;
    ISettingsController _iSettingsController { get { return _iSettingsControllerObj as ISettingsController; } }
    [SerializeField, RestrictToType(typeof(ISettable))] List<Object> _settable;
    List<ISettable> _iSettable {
        get {
            List<ISettable> i = new List<ISettable>();
            foreach (var o in _settable) {
                i.Add(o as ISettable);
            }
            return i;
        }
    }


    void Awake() {
        InitialiseSingleton();
    }

    void InitialiseSingleton() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DoSettingRequestActions();
    }

    void DoSettingRequestActions() {
        foreach (ISettable item in _iSettable) {
            UnityEvent<SettingsData> e = _iSettingsController.Subscribe(item.SettingsKey);
            e.AddListener(item.RegisterSettings);
            SetItemInitialState(item);
        }
    }

    void SetItemInitialState(ISettable item) {
        SettingsData sd = _iSettingsController.Load(item.SettingsKey);
        item.RegisterSettings(sd);
    }

}
