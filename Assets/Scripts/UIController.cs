using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController Instance { get; private set; } = null;

    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject titlePanel;
    [SerializeField] Text versionText;
    [SerializeField] Text scoreText;
    [SerializeField] Text gameoverHighScoreText;
    [SerializeField] Text gameoverScoreText;
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] SettingsToggle[] settingsToggles;


    void Awake() {
        InitialiseSingleton();
    }

    void InitialiseSingleton() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        DoSettingsToggleStuff();
    }

    void DoSettingsToggleStuff() {
        for (int i = 0; i < settingsToggles.Length; i++) {
            SettingsToggle t = settingsToggles[i];
            
            settingsToggles[i].onValueChanged.AddListener(delegate {
                ToggleAction(t);
            });
            try {
                settingsToggles[i].SetIsOnWithoutNotify(Settings.Load(t.SettingsKey));
            } catch (UnityException) {
                Debug.LogError($"Unable to load key '{t.SettingsKey}'.");
            }
        }
    }

    void ToggleAction(SettingsToggle t) {
        Settings.Save(t.SettingsKey, t.isOn);
    }


    public void PlayButtonClick() {
        GameController.Instance.SetPlayActive();
    }

    public void RetryButtonClick() {
        GameController.Instance.RebootWithAds();
    }

    public void SetGamePanelActive(bool status) {
        gamePanel.SetActive(status);
    }

    public void SetTitlePanelActive(bool status) {
        titlePanel.SetActive(status);
    }

    public void SetGameOverPanelActive(bool status) {
        gameoverPanel.SetActive(status);
    }

    public void OpenMenuPanel() {
        menuPanel.SetActive(true);
    }

    public void CloseMenuPanel() {
        menuPanel.SetActive(false);
    }

    public void SetVersionText(string text) {
        versionText.text = text;
    }

    public void SetScoreText(string text) {
        scoreText.text = text;
    }

    public void SetGameoverScoreText(string text) {
        gameoverScoreText.text = text;
    }

    public void SetGameoverHighScoreText(string text) {
        gameoverHighScoreText.text = text;
    }
}






