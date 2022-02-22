using UnityEngine;

public enum CircleType {
    stroke, dash, fill
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteAlways]
public class CircleGenerator : MonoBehaviour {
    readonly int[] polygonQuantityArray = new int[] {3, 3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20, 24, 30, 36, 40, 45, 60, 72, 90, 120, 180, 360 };

    [Range(0, 10), SerializeField]
    float _size = 1;
    public float Size { get { return _size; } }

    [Range(0, 3), SerializeField]
    float _thickness = 0.5f;

    [Range(0, 360), SerializeField]
    int _completion = 360;
    public int Completion { get { return _completion; } set { _completion = value; } }

    [Range(0, 360), SerializeField]
    int _angle = 90;

    [Range(1,22), SerializeField]
    int _polygons = 1;

    [SerializeField]
    bool _keepPolygonAmountConsistentForCompletionAmount = false;

    [SerializeField]
    CircleType _type = CircleType.stroke;

    int _polygonQty = 0;

    Vector3[] _vertices;
    int[] _triangles;
    Mesh _mesh;

    void Start() {
        Generate();
    }

    public void Generate() {
        if(_mesh == null) {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _mesh.name = "Circle";
        }

        if (_completion == 0) {
            _mesh.Clear();
            return;
        }

        CalculatePolygonAmount();

        _mesh.Clear();

        SetMeshVertices();

        _mesh.RecalculateNormals();

        SetMeshTriangles();
    }

    void CalculatePolygonAmount() {
        _polygonQty = polygonQuantityArray[_polygons];

        if (_keepPolygonAmountConsistentForCompletionAmount) {
            _polygonQty *= (int)Mathf.Ceil((_completion * Mathf.Deg2Rad));
        }
    }

    void SetMeshVertices() {
        CreateVerticesArray();

        for (int i = 0; i <= _polygonQty; i++) {

            float rad = i * -((float)_completion / _polygonQty) * Mathf.Deg2Rad + (_angle * Mathf.Deg2Rad);

            float x = Mathf.Cos(rad) * _thickness * _size;
            float y = Mathf.Sin(rad) * _thickness * _size;

            if (_type != CircleType.fill) {
                
                
                _vertices[i] = new Vector2(x, y);
                 x = Mathf.Cos(rad) * _size;
                 y = Mathf.Sin(rad) * _size;
                SetVertexPosition(i, new Vector2(x, y));
            } else {
                    _vertices[0] = new Vector2(0, 0);
                
                    SetVertexPosition(i, new Vector2(x, y));
                
            }

            
        }
        _mesh.vertices = _vertices;
    }

    void CreateVerticesArray() {
        if (_type == CircleType.stroke) {
            _vertices = new Vector3[(_polygonQty + 1) * 2];
        } else if (_type == CircleType.dash) {
            _vertices = new Vector3[_polygonQty * 4];
        } else if (_type == CircleType.fill) {
            _vertices = new Vector3[_polygonQty + 2];
        }
    }

    void SetVertexPosition(int index, Vector2 position) {
        if (_type == CircleType.stroke) {
            _vertices[index + _polygonQty + 1] = position;
        } else if (_type == CircleType.dash) {
            _vertices[index + (_polygonQty * 2)] = position;
        } else if(_type == CircleType.fill) {
            _vertices[index + 1] = position;
        }
    }

    void SetMeshTriangles() {
        _triangles = new int[_polygonQty * GetTriangleIndexIncrement()];
        for (int ti = 0, vi = 0, x = 0; x < _polygonQty; x++, ti += GetTriangleIndexIncrement(), vi += GetVertexIndexIncrement()) {
            SetTriangle(ti,vi);
            _mesh.triangles = _triangles;
        }
    }

    void SetTriangle(int triangleIndex, int vertexIndex) {
        int ti = triangleIndex;
        int vi = vertexIndex;
        if (_type != CircleType.fill) {
            _triangles[ti] = vi;
            _triangles[ti + 1] = _triangles[ti + 4] = vi + 1;
            if (_type == CircleType.stroke) {
                _triangles[ti + 2] = _triangles[ti + 3] = vi + _polygonQty + 1;
                _triangles[ti + 5] = vi + _polygonQty + 2;
            } else if (_type == CircleType.dash) {
                _triangles[ti + 2] = _triangles[ti + 3] = vi + (_polygonQty * 2);
                _triangles[ti + 5] = vi + (_polygonQty * 2) + 1;
            }
        } else {
            _triangles[ti] = 0;
            _triangles[ti+1] = vi + 1;
            _triangles[ti + 2] = vi + 2;
        }
    }

    int GetVertexIndexIncrement() {
        if(_type == CircleType.dash) {
            return 2;
        }
        return 1;
    }

    int GetTriangleIndexIncrement() {
        if (_type == CircleType.fill) {
            return 3;
        }
        return 6;
    }




#if UNITY_EDITOR
    private void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

    private void _OnValidate() {
        UnityEditor.EditorApplication.delayCall -= _OnValidate;
        if (this == null) return;
        Generate();
    }
#endif

}
