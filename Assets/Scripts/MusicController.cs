using UnityEngine;

public class MusicController : SettingsRequester {
    public static MusicController Instance { get; private set; } = null;

    AudioSource _audioSource;

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
        if (SettingsState == true && !_audioSource.isPlaying) {
            _audioSource.Play();
        }
    }

    protected override void RegisterSettings() {
        if (SettingsState == true) {
            _audioSource.Play();
        } else {
            _audioSource.Stop();
        }
    }




}
