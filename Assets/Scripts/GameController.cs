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
    public bool playing { get; set; } = false;


    public Text scoreText;

    public GameObject gameoverPanel;
    public Text gameoverScoreText;
    public Button retryButton;

    public GameObject gamePanel;
    public GameObject gameStartPanel;
    public GameObject aboutPanel;

    Random.State randomState;


    public delegate void MyDelegate();
    MyDelegate myDelegate;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }

        Time.timeScale = timescale;
        Random.InitState (randomSeed);
        randomState = Random.state;

	}

    void Start() {
        timeSinceAd = Time.unscaledTime;
        GetHighScore();
        UpdateUI();
        Player.Instance.OnPlatformLanded += DoPlayerPlatformLandedActions;
    }

    void DoPlayerPlatformLandedActions(int platformId) {
        UpdateScoreBasedOnPlatformId(platformId);
    }

    void UpdateScoreBasedOnPlatformId(int platformId) {
        if (platformId > Score) {
            SetScore(platformId);
        }
    }

    
    void SetScore(int score) {
        this.score = score;
        UpdateUI ();
    }

    public void Die() {
        lives--;
        if (lives < 1) {
            SaveHighScore();
            DeactivatePlayer();
            ShowGameoverPanel();
        } else {
            UpdateUI();
        }
    }

    void DeactivatePlayer() {
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.gameObject.transform.position = new Vector3(0, -100, 0);
    }

    void ShowGameoverPanel() {
        gamePanel.SetActive(false);
        gameoverScoreText.text = "Score: " + score.ToString() + ", Best: " + highScore.ToString() + ".";
        gameoverPanel.SetActive(true);
    }

    void UpdateUI() {
        scoreText.text = score.ToString ();
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
        yield return new WaitUntil(() => Camera.main.transform.position.x == CameraController.instance.TitleScreenPosition);
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

    public void PlayButtonClick() {
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
