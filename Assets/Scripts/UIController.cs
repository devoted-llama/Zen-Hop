using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField, RestrictToType(typeof(ISettingsController))] Object _iSettingsControllerObj;
    ISettingsController _iSettingsController { get { return _iSettingsControllerObj as ISettingsController; } }
    [SerializeField, RestrictToType(typeof(IChangeableSettingsElement))] List<Object> _changeableSettingsElementObj;
    List<IChangeableSettingsElement> _changeableSettingsElement {
        get {
            List<IChangeableSettingsElement> i = new List<IChangeableSettingsElement>();
            foreach (var o in _changeableSettingsElementObj) i.Add(o as IChangeableSettingsElement);
            return i;
        }
    }

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
        DoSettingsElementActions();
    }

    void DoSettingsElementActions() {
        foreach (IChangeableSettingsElement element in _changeableSettingsElement) {
            element.AddListener(delegate {SetElementState(element);});
            SetElementInitialState(element);
        }
    }

    void SetElementState(IChangeableSettingsElement element) {
        _iSettingsController.Save(element.SettingsKey, element.SettingsData);
    }

    void SetElementInitialState(IChangeableSettingsElement element) {
        try {
            element.SetSettingsData(_iSettingsController.Load(element.SettingsKey));
        } catch (UnityException) {
            Debug.LogError($"Unable to load key '{element.SettingsKey}'.");
        }
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
