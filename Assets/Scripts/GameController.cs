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

    public GameObject gameoverPanel;
    public Text gameoverScoreText;
    public Button retryButton;

    public GameObject gamePanel;
    public GameObject gameStartPanel;

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

    void UpdateUI() {
        scoreText.text = score.ToString ();
        //livesText.text = lives.ToString ();
    }

    public void Reboot() {
        ResetScore();
        ResetLives();
        UpdateUI();
        //AdController.instance.ShowAd();
        Random.state = randomState;
        PlatformController.instance.GeneratePlatforms();
        Frog.instance.gameObject.SetActive(true);
        Frog.instance.Respawn();
        gameoverPanel.SetActive(false);
    }

    public void ResetToTitleScreen() {
        StartCoroutine(ResetToTitleScreenCoroutine());
    }

    IEnumerator ResetToTitleScreenCoroutine() {
        playing = false;
        ResetScore();
        ResetLives();
        UpdateUI();
        gameoverPanel.SetActive(false);
        gamePanel.SetActive(false);
        CameraController.instance.MoveToTitleScreenPosition();
        Frog.instance.ResetToStartPosition();
        Random.state = randomState;
        PlatformController.instance.GeneratePlatforms();
        yield return new WaitUntil(() => Camera.main.transform.position.x == CameraController.titleScreenPosition);
        gameStartPanel.SetActive(true);
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

    public void Play() {
        CameraController.instance.finishMoving += SetPlayActive;
        CameraController.instance.MoveTo(PlatformController.instance.GetPlatformById(0).transform, 5);
        gameStartPanel.SetActive(false);

    }

    void SetPlayActive() {
        Frog.instance.gameObject.SetActive(true);
        gamePanel.SetActive(true);
        playing = true;
        CameraController.instance.finishMoving -= SetPlayActive;
    }
}
