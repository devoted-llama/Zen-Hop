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
    [SerializeField] float initialScaleVariation = 1.5f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start() {
        Initialise();
    }

    private void OnEnable() {
        Initialise();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    void Initialise() {
        SetScale();
        SetForce();
        StartCoroutine(ShiftForce());
        StartCoroutine(ShiftScale());
    }

    void SetScale() {
        Vector3 scale = transform.localScale;
        float randomScaleVariation = Random.Range(1, 1 * initialScaleVariation);
        scale.x *= randomScaleVariation;
        scale.y *= randomScaleVariation;
        transform.localScale = scale;
        originalScale = transform.localScale;
        currentScale = originalScale;
    }

    private void Update() {
        LerpScale();
    }

    void LerpScale() {
        float timeDif = Time.time - startTime;
        float t = timeDif / scaleShiftTime;
        transform.localScale = Vector3.Lerp(currentScale, newScale, t);
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

    void SetForce() {
        Vector2 force = new Vector2(Random.Range(-forceVariation, forceVariation), Random.Range(-forceVariation, forceVariation));
        rb.AddForce(force);
    }
}
