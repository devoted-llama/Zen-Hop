using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float timescale = 1;
    public int randomSeed = 0;
    public float respawnTime = 1;
    public float respawnHeight = 5;
    public float fallSpeed = 10;

    public float level = 0;
    public float lives = 3;

    public Text scoreText;
    public Text livesText;


	void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        //DontDestroyOnLoad (gameObject);

        Time.timeScale = timescale;
        Random.InitState (randomSeed);

	}

    public void NewScore(int score) {
        level = score;
        UpdateUI ();
    }

    public void Die() {
        lives--;
        if (lives < 1) {
            Reboot ();
        } else {
            UpdateUI ();
        }
    }

    void UpdateUI() {
        scoreText.text = level.ToString ("000000");
        livesText.text = lives.ToString ();
    }

    void Reboot() {
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
    }
}
