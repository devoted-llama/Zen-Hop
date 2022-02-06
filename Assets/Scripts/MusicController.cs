using UnityEngine;

public class MusicController : MonoBehaviour {
    public static MusicController Instance { get; private set; } = null;

    AudioSource audioSource;
    public bool MusicPreference { get; private set; } = true;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        LoadMusicPreference();
    }

    public void PlayIfHasPreference() {
        if (MusicPreference == true) {
            audioSource.Play();
        }
    }

    public void ChangeMusicStateAndSetPreference() {
        if(audioSource.isPlaying != true) {
            audioSource.Play();
            SetMusicPreference(true);
        } else {
            audioSource.Stop();
            SetMusicPreference(false);
        }
    }

    void LoadMusicPreference() {
        if (PlayerPrefs.HasKey("musicPreference")) {
            MusicPreference = PlayerPrefs.GetInt("musicPreference") == 0 ? false : true;
        }
    }

    void SetMusicPreference(bool preference) {
        PlayerPrefs.SetInt("musicPreference", preference == false ? 0 : 1);
        PlayerPrefs.Save();
    }


}
