
using UnityEngine.Advertisements;
using UnityEngine;


public class AdController : MonoBehaviour, IUnityAdsListener {
    public static AdController instance = null;

    string gameId = "1234567";
    bool testMode = true;

    public delegate void AdEvent();
    public event AdEvent AdFinished;

    void Start() {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void ShowAd() {
        if (Advertisement.IsReady()) {
            Advertisement.Show();
        } else {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    void IUnityAdsListener.OnUnityAdsReady(string placementId) {
        
    }

    void IUnityAdsListener.OnUnityAdsDidError(string message) {
        Debug.LogError(message);
    }

    void IUnityAdsListener.OnUnityAdsDidStart(string placementId) {
        Debug.Log("The ad started playing.");
    }

    void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        if(showResult == ShowResult.Finished) {
            AdFinished?.Invoke();
        } else if (showResult == ShowResult.Skipped) {
            Debug.Log("The ad was skipped");
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
        
    }
}
