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

    float score = 0;
    public float Score { get { return score; } }
    float lives = 1;
    public float Lives { get { return lives; } }
    float best = 0;
 

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

        DontDestroyOnLoad (gameObject);

        Time.timeScale = timescale;
        Random.InitState (randomSeed);

	}

    void Start() {
        UpdateUI();
    }

    public void NewScore(int score) {
        this.score = score;
        UpdateUI ();
    }

    public void Die() {
        AudioController.instance.PlayDeathSound();
        lives--;
        if (lives < 1) {
            ShowGameoverPanel();
        } else {
            UpdateUI();
        }
    }

    void ShowGameoverPanel() {
        best = score > best ? score : best;
        Frog.instance.gameObject.SetActive(false);
        gameoverScoreText.text = "Score: " + score.ToString() + ", Best: " + best.ToString() + ".";
        gameoverPanel.SetActive(true);
    }

    void HideGameoverPanel() {
        gameoverPanel.SetActive(false);
    }

    void UpdateUI() {
        scoreText.text = score.ToString ();
        //livesText.text = lives.ToString ();
    }

    public void Reboot() {
        
        score = 0;
        lives = 1;
        UpdateUI();
        //AdController.instance.ShowAd();
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        HideGameoverPanel();
    }
}
