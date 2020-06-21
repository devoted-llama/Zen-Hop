using System.Collections;
using UnityEngine;
using UnityEditor;

public enum Direction {
    left, right
}

[ExecuteInEditMode,RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SineWave : MonoBehaviour {
    public Direction direction = Direction.right;

    [Range(0f, 1f)]
    public float amplitude = 0.2f;

    [Range(0f, 10f)]
    public float frequency = 0.1f;

    float Frequency {
        get {
            return direction == Direction.right ? 0 - frequency : frequency;
        }
    }

    [Range(0.01f, 100)]
    public float wavelength = 10f;

    float WaveLength {
        get {
            return 10000f / wavelength;
        }
    }

    float AngularFrequency {
        get {
            return 2 * Mathf.PI * Frequency;
        }
    }

    [SerializeField, Range(1, 1000)]
    int polygons = 100;

    public int Polygons {
        get {
            return polygons;
        }
        set {
            Generate();
        }
    }


    [Range(-0.5f, 0.5f)]
    public float height = 0;

    float updateTime = 0.01f;

    Vector3[] vertices;
    Mesh mesh;

    public float time = 0f;

    private void Awake() {
        Generate();
        StartCoroutine(Sinify());
    }

    IEnumerator Sinify() {
        while (true) {
            float deg = 0;
            for (int i = polygons + 1; i < vertices.Length; deg += WaveLength / polygons, i++) {
                float rad = Mathf.Deg2Rad * deg;
                float y = amplitude * Mathf.Sin((AngularFrequency * time) + rad);
                vertices[i].y = y + height;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            yield return new WaitForSeconds(updateTime);
            time += updateTime;
        }
    }

    void Generate() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr.sharedMaterial == null) {
            mr.sharedMaterial = Resources.Load<Material>("SineWave");
        }
        
        mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.name = "Sine Wave";

        vertices = new Vector3[(polygons + 1) * 2];

        float yStart = -0.5f;
        float yInc = 1;
        float yEnd = yStart + yInc;

        float xStart = -0.5f;
        float xInc = 1f / (float)polygons;
        float xEnd = xStart + (xInc * (float)polygons);

        float y = yStart;
        for (int i = 0, yi = 0; y <= yEnd; y += yInc, yi++) {
            for (float x = xStart; i < (polygons + 1) * (yi+1); x += xInc, i++) {
                vertices[i] = new Vector3(x, y);
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[polygons * 6];
        for (int ti = 0, vi = 0, x = 0; x < polygons; x++, ti += 6, vi++) {
            triangles[ti] = vi;
            triangles[ti + 1] = triangles[ti + 4] = vi + polygons + 1;
            triangles[ti + 2] = triangles[ti + 3] = vi + 1;
            triangles[ti + 5] = vi + polygons + 2;
            mesh.triangles = triangles;
        }
    }

    private void OnValidate() {
        Polygons = polygons;
    }

    [MenuItem("GameObject/2D Object/Sine Wave", false, 1000)]
    static void CreateCustomGameObject(MenuCommand menuCommand) {
        GameObject go = new GameObject("Sine Wave");
        go.AddComponent<SineWave>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

}
