using UnityEngine;
using UnityEngine.Advertisements;

public class AdInitialiser : MonoBehaviour, IUnityAdsInitializationListener {
    public static AdInitialiser Instance = null;

    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        InitializeAds();
    }

    public void InitializeAds() {
        _gameId = _iOSGameId;
        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete() {
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}