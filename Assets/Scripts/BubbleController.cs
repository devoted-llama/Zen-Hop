using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {
    public static BubbleController Instance { get; private set; } = null;
    [SerializeField] GameObject bubblePrefab;
    [SerializeField] float bubbleSizeVariation;
    [SerializeField] int quantity;
    [SerializeField] float boundarySize;
    [SerializeField] int boundaryEdges;
    EdgeCollider2D ec;

    public bool BackgroundPreference { get; private set; } = true;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }

        LoadMusicPreference();
    }

    private void Start() {
        ec = GetComponent<EdgeCollider2D>();
        SetUpColliderCircle();
        InstantiateBubbles();
        SetStateDependingOnPreference();
    }

    void SetStateDependingOnPreference() {
        gameObject.SetActive(BackgroundPreference);
    }

    void SetUpColliderCircle() {
        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i <= boundaryEdges; i++) {
            float rad = i * -(360f / boundaryEdges) * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * boundarySize;
            float y = Mathf.Sin(rad) * boundarySize;

            vertices.Add(new Vector2(x, y));
        }
        ec.SetPoints(vertices);
    }

    void InstantiateBubbles() {
        for (int i = 0; i <= quantity; i++) {
            GameObject bubble = Instantiate(bubblePrefab, transform);
            float positionVariation = boundarySize * 0.5f;
            bubble.transform.position = new Vector3(
                Random.Range(-positionVariation, positionVariation),
                Random.Range(-positionVariation, positionVariation), 0);
            Vector3 scale = bubble.transform.localScale;
            float scaleVariation = Random.Range(1 - bubbleSizeVariation, 1 + bubbleSizeVariation);
            scale.x *= scaleVariation;
            scale.y *= scaleVariation;
            bubble.transform.localScale = scale;
        }
    }

    public void ChangeBackgroundStateAndSetPreference() {
        if (gameObject.activeSelf != true) {
            gameObject.SetActive(true);
            SetBackgroundPreference(true);
        } else {
            gameObject.SetActive(false);
            SetBackgroundPreference(false);
        }
    }

    void LoadMusicPreference() {
        if (PlayerPrefs.HasKey("backgroundPreference")) {
            BackgroundPreference = PlayerPrefs.GetInt("backgroundPreference") == 0 ? false : true;
        }
    }

    void SetBackgroundPreference(bool preference) {
        PlayerPrefs.SetInt("backgroundPreference", preference == false ? 0 : 1);
        PlayerPrefs.Save();
    }
}
