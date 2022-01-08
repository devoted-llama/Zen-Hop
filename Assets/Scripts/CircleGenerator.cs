using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
    bool dashed = false;

    int polygonQty = 0;


    

    Vector3[] vertices;
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
        if(!dashed) { 
            vertices = new Vector3[(polygonQty + 1) * 2];
        } else {
            vertices = new Vector3[polygonQty * 4];
        }

        for (int i = 0; i <= polygonQty; i++) {

            float rad = i * -((float)completion / polygonQty) * Mathf.Deg2Rad + (angle * Mathf.Deg2Rad);

            float x = Mathf.Cos(rad) * size;
            float y = Mathf.Sin(rad) * size;
            if (!dashed) {
                vertices[i + polygonQty + 1] = new Vector2(x, y);
            } else {
                vertices[i + (polygonQty * 2)] = new Vector2(x, y);
            }

            x = Mathf.Cos(rad) * thickness * size;
            y = Mathf.Sin(rad) * thickness * size;
            vertices[i] = new Vector2(x, y);
        }
        mesh.vertices = vertices;
    }

    void SetMeshTriangles() {
        int[] triangles = new int[polygonQty * 6];
        for (int ti = 0, vi = 0, x = 0; x < polygonQty; x++, ti += 6, vi+=!dashed?1:2) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + 1;
            if (!dashed) {
                triangles[ti + 2] = triangles[ti + 3] = vi + polygonQty + 1;
                triangles[ti + 5] = vi + polygonQty + 2;
            } else {
                triangles[ti + 2] = triangles[ti + 3] = vi + (polygonQty * 2);
                triangles[ti + 5] = vi + (polygonQty * 2) + 1;
            }
            mesh.triangles = triangles;
        }
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
