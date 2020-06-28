using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircleGenerator : MonoBehaviour {
    readonly int[] polygonArray = new int[] {3, 3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20, 24, 30, 36, 40, 45, 60, 72, 90, 120, 180, 360 };

    [Range(0, 10)]
    public float size = 1;

    [Range(1, 3)]
    public float thickness = 0.5f;

    [Range(0, 360)]
    public int completion = 360;

    [Range(0, 360)]
    public int angle = 90;

    [Range(1,22)]
    public int polygons = 1;

    public bool keepPolygonAmountConsistent = false;
    

    Vector3[] vertices;
    Mesh mesh;

    private void OnValidate() {
        Generate();
    }

    void Awake() {
        Generate();
    }


    public void Generate() {
        if(mesh == null) {
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.name = "Circle";
        }

        if (completion == 0) {
            mesh.Clear();
            return;
        }

        int amt = polygonArray[polygons];
        if(keepPolygonAmountConsistent) {
            amt *= (int)Mathf.Ceil(((float)completion * Mathf.Deg2Rad));
        }

        vertices = new Vector3[(amt+1) * 2];

        for(int i = 0; i <= amt; i++) {
            float rad = i * ((float)completion / (float)amt) * Mathf.Deg2Rad + (this.angle * Mathf.Deg2Rad);
            
            float x = Mathf.Cos(rad) * size;
            float y = Mathf.Sin(rad) * size;

            vertices[i+amt+1] = new Vector2(x, y);

            x = Mathf.Cos(rad) * thickness * size;
            y = Mathf.Sin(rad) * thickness * size;
            vertices[i] = new Vector2(x, y);
        }

        MeshRenderer mr = GetComponent<MeshRenderer>();

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.RecalculateNormals();

        int[] triangles = new int[amt * 6];
        for (int ti = 0, vi = 0, x = 0; x < amt; x++, ti += 6, vi++) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + amt + 1;
            triangles[ti + 2] = triangles[ti + 3] = vi + 1;
            triangles[ti + 5] = vi + amt + 2;
            mesh.triangles = triangles;
        }
    }

    [MenuItem("GameObject/2D Object/Circle", false, 1000)]
    static void CreateCustomGameObject(MenuCommand menuCommand) {
        GameObject go = new GameObject("Circle");
        go.AddComponent<CircleGenerator>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

}
