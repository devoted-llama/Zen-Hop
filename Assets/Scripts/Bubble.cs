using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour {

    Rigidbody2D rb;
    Vector3 originalScale;
    Vector3 currentScale;
    Vector3 newScale;
    float startTime;
    [SerializeField] float scaleShiftMultiplier = 1.5f;
    [SerializeField] float scaleShiftTime = 30f;
    [SerializeField]float ShiftForceTime = 30f;
    [SerializeField] float forceVariation = 0.1f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start() {
        originalScale = transform.localScale;
        currentScale = originalScale;
        Initialise();
    }

    void Initialise()  {
        SetForce();
        StartCoroutine(ShiftForce());
        StartCoroutine(ShiftScale());
    }

    private void OnEnable() {
        Initialise();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    private void Update() {
        LerpScale();
    }

    void SetForce() {
        Vector2 force = new Vector2(Random.Range(-forceVariation, forceVariation), Random.Range(-forceVariation, forceVariation));
        rb.AddForce(force);
    }

    /* This is in case the bubble gets stuck somewhere */
    IEnumerator ShiftForce() {
        while(true) {
            yield return new WaitForSeconds(ShiftForceTime);
            SetForce();
        }
    }

    IEnumerator ShiftScale() {
        while (true) {
            
            float randomScale = Random.Range(1, scaleShiftMultiplier);
            newScale.x = originalScale.x * randomScale;
            newScale.y = originalScale.y * randomScale;
            currentScale = transform.localScale;
            startTime = Time.time;
            yield return new WaitForSeconds(scaleShiftTime);
        }
    }

    void LerpScale() {
        float timeDif = Time.time - startTime;
        float t = timeDif / scaleShiftTime;
        transform.localScale = Vector3.Lerp(currentScale, newScale, t);
    }

}
