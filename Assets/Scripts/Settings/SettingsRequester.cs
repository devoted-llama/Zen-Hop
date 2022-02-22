using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SettingsRequester : MonoBehaviour {
    public static SettingsRequester Instance { get; private set; } = null;

    [SerializeField, RestrictToType(typeof(ISettingsController))] Object _iSettingsControllerObj;
    ISettingsController _iSettingsController { get { return _iSettingsControllerObj as ISettingsController; } }
    [SerializeField, RestrictToType(typeof(ISettable<bool>))] List<Object> _settable;
    List<ISettable<bool>> _iSettable {
        get {
            List<ISettable<bool>> i = new List<ISettable<bool>>();
            foreach (var o in _settable) {
                i.Add(o as ISettable<bool>);
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
        DoBoolSettingRequestActions();
    }

    void DoBoolSettingRequestActions() {
        foreach (ISettable<bool> item in _iSettable) {
            UnityEvent<bool> e = _iSettingsController.SubscribeToBool(item.SettingsKey);
            e.AddListener(item.RegisterSettings);
            SetItemInitialState(item);
        }
    }

    void SetItemInitialState(ISettable<bool> item) {
        bool value = _iSettingsController.LoadBool(item.SettingsKey);
        item.RegisterSettings(value);
    }

}
