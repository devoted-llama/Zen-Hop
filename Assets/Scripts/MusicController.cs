using UnityEngine;

public class MusicController : MonoBehaviour, ISettable {
    public static MusicController Instance { get; private set; } = null;

    AudioSource _audioSource;
    bool _settingsState;

    [SerializeField] string _settingsKey = "sound";
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }

    void Awake() {
        InitialiseSingleton();
        _audioSource = GetComponent<AudioSource>();
    }

    void InitialiseSingleton() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    public void PlayIfHasPreference() {
        if (_settingsState == true && !_audioSource.isPlaying) {
            _audioSource.Play();
        }
    }

    void SetMusicBasedOnState() {
        if (_settingsState == true) {
            _audioSource.Play();
        } else {
            _audioSource.Stop();
        }
    }

    public void RegisterSettings(SettingsData sd) {
        _settingsState = sd.Bool;
        SetMusicBasedOnState();
    }




}
