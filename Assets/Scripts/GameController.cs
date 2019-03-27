using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour {
    public static GameController instance = null;

    public float timescale;
    public int randomSeed;
    public float respawnTime;
    public float respawnHeight;
    public float fallSpeed;

    [SerializeField]
    int score = 0;
    public int Score { get { return score; } }
    [SerializeField]
    int lives = 1;
    public int Lives { get { return lives; } }
    [SerializeField]
    int highScore = 0;

    bool _playing = false;
    public bool playing {  get { return _playing; } set { _playing = value; } }
 

    public Text scoreText;
    public Text livesText;

    public GameObject gameoverPanel;
    public Text gameoverScoreText;
    public Button retryButton;

    Random.State randomState;

	void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        //DontDestroyOnLoad (gameObject);

        Time.timeScale = timescale;
        Random.InitState (randomSeed);
        randomState = Random.state;

	}

    void Start() {
        GetHighScore();
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
        SaveHighScore();
        Frog.instance.gameObject.SetActive(false);
        gameoverScoreText.text = "Score: " + score.ToString() + ", Best: " + highScore.ToString() + ".";
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
        ResetScore();
        ResetLives();
        UpdateUI();
        //AdController.instance.ShowAd();
        //SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        Random.state = randomState;
        PlatformController.instance.GeneratePlatforms();
        Frog.instance.gameObject.SetActive(true);
        Frog.instance.Respawn();
        HideGameoverPanel();
    }

    void SaveHighScore() {
        if(score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }
    }

    void GetHighScore() {
        if(PlayerPrefs.HasKey("highScore")) {
            highScore = PlayerPrefs.GetInt("highScore");
        }
    }

    void ResetScore() {
        score = 0;
    }

    void ResetLives() {
        lives = 1;
    }
}
