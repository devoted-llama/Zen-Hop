
using UnityEngine.Advertisements;
using UnityEngine;


public class AdInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    public static AdInterstitial instance = null;

    [SerializeField] string _iOsAdUnitId;
    string _adUnitId;

    public delegate void AdEvent();
    public event AdEvent AdFinished;



    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DoInit(); 
    }

    void DoInit() {
        _adUnitId = _iOsAdUnitId;
    }

    public void LoadAd() {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }


    public void ShowAd() {
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId) {
        
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) {
        Debug.LogError(message);
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }



    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) {
        AdFinished();
    }
}
