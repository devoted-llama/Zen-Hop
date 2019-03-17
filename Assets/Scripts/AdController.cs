
using System.Collections;
using UnityEngine.Monetization;
using UnityEngine;

public class AdController : MonoBehaviour
{
    public static AdController instance = null;

    public string placementId = "video";

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void ShowAd() {
        StartCoroutine(ShowAdWhenReady());
    }

    private IEnumerator ShowAdWhenReady() {
        while (!Monetization.IsReady(placementId)) {
            yield return new WaitForSeconds(0.25f);
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        if (ad != null) {
            ad.Show();
        }
    }
}
