using UnityEngine;

public enum CircleType {
    stroke, dash, fill
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteAlways]
public class CircleGenerator : MonoBehaviour {
    readonly int[] polygonQuantityArray = new int[] {3, 3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20, 24, 30, 36, 40, 45, 60, 72, 90, 120, 180, 360 };

    [Range(0, 10), SerializeField]
    float size = 1;

    public float Size { get { return size; } }

    [Range(0, 3), SerializeField]
    float thickness = 0.5f;

    [Range(0, 360), SerializeField]
    int completion = 360;

    public int Completion { get { return completion; } set { completion = value; } }

    [Range(0, 360), SerializeField]
    int angle = 90;

    [Range(1,22), SerializeField]
    int polygons = 1;

    [SerializeField]
    bool keepPolygonAmountConsistentForCompletionAmount = false;

    [SerializeField]
    CircleType type = CircleType.stroke;

    int polygonQty = 0;

    Vector3[] vertices;
    int[] triangles;
    Mesh mesh;

    void Start() {
        Generate();
    }

    public void Generate() {
        if(mesh == null) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            mesh.name = "Circle";
        }

        if (completion == 0) {
            mesh.Clear();
            return;
        }

        CalculatePolygonAmount();

        mesh.Clear();

        SetMeshVertices();

        mesh.RecalculateNormals();

        SetMeshTriangles();
    }

    void CalculatePolygonAmount() {
        polygonQty = polygonQuantityArray[polygons];

        if (keepPolygonAmountConsistentForCompletionAmount) {
            polygonQty *= (int)Mathf.Ceil((completion * Mathf.Deg2Rad));
        }
    }

    void SetMeshVertices() {
        CreateVerticesArray();

        for (int i = 0; i <= polygonQty; i++) {

            float rad = i * -((float)completion / polygonQty) * Mathf.Deg2Rad + (angle * Mathf.Deg2Rad);

            float x = Mathf.Cos(rad) * thickness * size;
            float y = Mathf.Sin(rad) * thickness * size;

            if (type != CircleType.fill) {
                
                
                vertices[i] = new Vector2(x, y);
                 x = Mathf.Cos(rad) * size;
                 y = Mathf.Sin(rad) * size;
                SetVertexPosition(i, new Vector2(x, y));
            } else {
                    vertices[0] = new Vector2(0, 0);
                
                    SetVertexPosition(i, new Vector2(x, y));
                
            }

            
        }
        mesh.vertices = vertices;
    }

    void CreateVerticesArray() {
        if (type == CircleType.stroke) {
            vertices = new Vector3[(polygonQty + 1) * 2];
        } else if (type == CircleType.dash) {
            vertices = new Vector3[polygonQty * 4];
        } else if (type == CircleType.fill) {
            vertices = new Vector3[polygonQty + 2];
        }
    }

    void SetVertexPosition(int index, Vector2 position) {
        if (type == CircleType.stroke) {
            vertices[index + polygonQty + 1] = position;
        } else if (type == CircleType.dash) {
            vertices[index + (polygonQty * 2)] = position;
        } else if(type == CircleType.fill) {
            vertices[index + 1] = position;
        }
    }

    void SetMeshTriangles() {
        triangles = new int[polygonQty * GetTriangleIndexIncrement()];
        for (int ti = 0, vi = 0, x = 0; x < polygonQty; x++, ti += GetTriangleIndexIncrement(), vi += GetVertexIndexIncrement()) {
            SetTriangle(ti,vi);
            mesh.triangles = triangles;
        }
    }

    void SetTriangle(int triangleIndex, int vertexIndex) {
        int ti = triangleIndex;
        int vi = vertexIndex;
        if (type != CircleType.fill) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + 1;
            if (type == CircleType.stroke) {
                triangles[ti + 2] = triangles[ti + 3] = vi + polygonQty + 1;
                triangles[ti + 5] = vi + polygonQty + 2;
            } else if (type == CircleType.dash) {
                triangles[ti + 2] = triangles[ti + 3] = vi + (polygonQty * 2);
                triangles[ti + 5] = vi + (polygonQty * 2) + 1;
            }
        } else {
            triangles[ti] = 0;
            triangles[ti+1] = vi + 1;
            triangles[ti + 2] = vi + 2;
        }
    }

    int GetVertexIndexIncrement() {
        if(type == CircleType.dash) {
            return 2;
        }
        return 1;
    }

    int GetTriangleIndexIncrement() {
        if (type == CircleType.fill) {
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
