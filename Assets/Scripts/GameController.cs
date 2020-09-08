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
    public float timeBetweenAds;

    [SerializeField]
    float timeSinceAd = 0;
    public float TimeSinceAd { get { return timeSinceAd; } }

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
    public GameObject aboutPanel;

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
        timeSinceAd = Time.unscaledTime;
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
        
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.gameObject.transform.position = new Vector3(0, -100, 0);
        gamePanel.SetActive(false);
        gameoverScoreText.text = "Score: " + score.ToString() + ", Best: " + highScore.ToString() + ".";
        gameoverPanel.SetActive(true);
    }

    void UpdateUI() {
        scoreText.text = score.ToString ();
        //livesText.text = lives.ToString ();
    }



    public void RebootWithAds() {
        if (AdController.instance == null || Time.unscaledTime - TimeSinceAd < timeBetweenAds) {
            Reboot();
        } else {
            AdController.instance.AdFinished += Reboot;
            AdController.instance.ShowAd();
            timeSinceAd = Time.unscaledTime;
        }
    }

    void Reboot() {
        ResetScore();
        ResetLives();
        UpdateUI();
        Random.state = randomState;
        PlatformController.Instance.PositionStartingPlatforms();
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.Respawn();
        gameoverPanel.SetActive(false);
        gamePanel.SetActive(true);
        if (AdController.instance != null) {
            AdController.instance.AdFinished -= Reboot;
        }
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
        Player.Instance.ResetToStartPosition();
        Random.state = randomState;
        PlatformController.Instance.PositionStartingPlatforms();
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
        CameraController.instance.MoveTo(PlatformController.Instance.GetPlatformById(0).transform);
        gameStartPanel.SetActive(false);

    }

    void SetPlayActive() {
        Player.Instance.gameObject.SetActive(true);
        gamePanel.SetActive(true);
        playing = true;
        CameraController.instance.finishMoving -= SetPlayActive;
    }

    public void ShowAboutScreen() {
        aboutPanel.SetActive(true);
    }

    public void HideAboutScreen() {
        aboutPanel.SetActive(false);
    }
}
