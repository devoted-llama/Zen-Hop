using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController instance = null;
    public AudioSource[] jumpSound;
    public AudioSource deathSound;

    [SerializeField]
    bool sound = true;
    public bool Sound { get { return sound; } }

    public delegate void SoundEvent(bool state);
    public event SoundEvent ToggleSoundEvent;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    public void PlayJumpSound() {
        if (sound) {
            jumpSound[Random.Range(0, jumpSound.Length)].Play();
        }
    }

    public void PlayDeathSound() {
        if (sound) {
            deathSound.Play();
        }
    }

    public void ToggleSound() {
        sound = !sound;
        if (ToggleSoundEvent != null) {
            ToggleSoundEvent(sound);
        }
    }

}
