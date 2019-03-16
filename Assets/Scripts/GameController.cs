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

    public AudioSource[] jumpSound;
    public AudioSource deathSound;

    public Text scoreText;
    public Text livesText;

    public GameObject gameoverPanel;
    public Text gameoverScoreText;
    public Button retryButton;

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

    void Start() {
        UpdateUI();
    }

    public void PlayJumpSound() {
        jumpSound[Random.Range(0, jumpSound.Length)].Play();
    }

    public void NewScore(int score) {
        level = score;
        UpdateUI ();
    }

    public void Die() {
        deathSound.Play();
        lives--;
        if (lives < 1) {
            ShowGameoverPanel();
        } else {
            UpdateUI ();
        }
    }

    void ShowGameoverPanel() {
        Frog.instance.gameObject.SetActive(false);
        gameoverScoreText.text = "You reached cloud " + level.ToString() + "!";
        gameoverPanel.SetActive(true);
    }

    void UpdateUI() {
        scoreText.text = level.ToString ();
        //livesText.text = lives.ToString ();
    }

    public void Reboot() {
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
    }
}
