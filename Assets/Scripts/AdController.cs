
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine;

public class AdController : MonoBehaviour
{
    public static AdController instance = null;

    public string placementId = "video";

    string gameId = "1234567";
    bool testMode = true;

    public delegate void AdEvent();
    public event AdEvent AdFinished;


    void Start() {
        Advertisement.Initialize(gameId, testMode);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_LEFT);
        StartCoroutine(ShowBannerWhenReady());
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void ShowAd() {
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(options);
    }

    void HandleShowResult(ShowResult result) {
        if (AdFinished != null) {
            AdFinished();
        }
    }

    IEnumerator ShowBannerWhenReady() {
        while (!Advertisement.IsReady(placementId)) {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(placementId);
    }
}
