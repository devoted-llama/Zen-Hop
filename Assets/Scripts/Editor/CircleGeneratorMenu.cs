using UnityEngine;
using UnityEditor;

public class CircleGeneratorMenu {
    [MenuItem("GameObject/2D Object/Circle", false, 1000)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Circle");
        go.AddComponent<CircleGenerator>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
