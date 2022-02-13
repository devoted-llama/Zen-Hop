using System.Collections.Generic;
using UnityEngine;

public class BubbleController : SettingsListener {
    public static BubbleController Instance { get; private set; } = null;
    [SerializeField] GameObject bubblePrefab;
    [SerializeField] int quantity;
    [SerializeField] float boundarySize;
    [SerializeField] int boundaryEdges;
    EdgeCollider2D ec;

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

    new void Start() {
        base.Start();
        ec = GetComponent<EdgeCollider2D>();
        SetUpColliderCircle();
        InstantiateBubbles();
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
        }
    }

    protected override void RegisterSettings() {
        gameObject.SetActive(SettingsState);
    }

}
