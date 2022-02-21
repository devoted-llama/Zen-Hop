using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour, ISettable {
    public static BubbleController Instance { get; private set; } = null;
    [SerializeField] GameObject _bubblePrefab;
    [SerializeField] int _quantity = 8;
    [SerializeField] float _boundarySize = 60;
    [SerializeField] int _boundaryEdges = 8;
    [SerializeField] string _settingsKey = "background";
    public string SettingsKey { get { return _settingsKey; } set { _settingsKey = value; } }
    EdgeCollider2D _ec;

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
        _ec = GetComponent<EdgeCollider2D>();
        SetUpColliderCircle();
        InstantiateBubbles();
    }

    void SetUpColliderCircle() {
        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i <= _boundaryEdges; i++) {
            float rad = i * -(360f / _boundaryEdges) * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * _boundarySize;
            float y = Mathf.Sin(rad) * _boundarySize;

            vertices.Add(new Vector2(x, y));
        }
        _ec.SetPoints(vertices);
    }

    void InstantiateBubbles() {
        for (int i = 0; i <= _quantity; i++) {
            GameObject bubble = Instantiate(_bubblePrefab, transform);
            float positionVariation = _boundarySize * 0.5f;
            bubble.transform.position = new Vector3(
                Random.Range(-positionVariation, positionVariation),
                Random.Range(-positionVariation, positionVariation), 0);
        }
    }

    public void RegisterSettings(SettingsData sd) {
        gameObject.SetActive(sd.Bool);
    }

}
