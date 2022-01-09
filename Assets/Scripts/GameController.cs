﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    public VersionInfo versionInfo;

    Random.State randomState;

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
        SetVersionText();
        Player.Instance.OnPlatformLanded += DoPlayerPlatformLandedActions;
    }

    void DoPlayerPlatformLandedActions(int platformId) {
        UpdateScoreBasedOnPlatformId(platformId);
        CameraController.Instance.MoveToPlayerExact();
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
            SetScoreText();
            ShowGameoverPanel();
        } else {
            UpdateUI();
        }
    }

    void DeactivatePlayer() {
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.gameObject.transform.position = new Vector3(0, -100, 0);
    }

    void SetScoreText() {
        UIController.Instance.SetGameoverScoreText(score.ToString());
        UIController.Instance.SetGameoverHighScoreText(highScore.ToString());
    }

    void ShowGameoverPanel() {
        UIController.Instance.SetGamePanelActive(false);
        UIController.Instance.SetGameOverPanelActive(true);
    }

    void UpdateUI() {
        UIController.Instance.SetScoreText(score.ToString());
    }



    public void RebootWithAds() {
        if (AdInterstitial.instance == null || Time.unscaledTime - TimeSinceAd < timeBetweenAds) {
            Reboot();
        } else {
            AdInterstitial.instance.AdFinished += Reboot;
            AdInterstitial.instance.LoadAd();
            AdInterstitial.instance.ShowAd();
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
        UIController.Instance.SetGameOverPanelActive(false);
        UIController.Instance.SetGamePanelActive(true);
        if (AdInterstitial.instance != null) {
            AdInterstitial.instance.AdFinished -= Reboot;
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
        UIController.Instance.SetGameOverPanelActive(false);
        UIController.Instance.SetGamePanelActive(false);
        CameraController.Instance.MoveToTitleScreenPosition();
        Player.Instance.ResetToStartPosition();
        Random.state = randomState;
        PlatformController.Instance.PositionStartingPlatforms();
        yield return new WaitUntil(() => Camera.main.transform.position.x == CameraController.Instance.TitleScreenPosition);
        UIController.Instance.SetTitlePanelActive(true);
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

    public void SetPlayActive() {
        Player.Instance.gameObject.SetActive(true);
        UIController.Instance.SetGamePanelActive(true);
        UIController.Instance.SetTitlePanelActive(false);
        playing = true;
    }

    void SetVersionText() {
        UIController.Instance.SetVersionText($"Version {versionInfo.version}.{versionInfo.buildNumber}");
    }
}
