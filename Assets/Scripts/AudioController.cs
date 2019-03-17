using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance = null;
    public AudioSource[] jumpSound;
    public AudioSource deathSound;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void PlayJumpSound() {
        jumpSound[Random.Range(0, jumpSound.Length)].Play();
    }

    public void PlayDeathSound() {
        deathSound.Play();
    }

}
