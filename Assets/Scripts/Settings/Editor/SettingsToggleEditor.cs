using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
[CustomEditor(typeof(SettingsToggle))]
public class SettingsToggleEditor : ToggleEditor {

    SerializedProperty _settingsKey;

    protected override void OnEnable() {
        base.OnEnable();
        _settingsKey = serializedObject.FindProperty("_settingsKey");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(_settingsKey, new GUIContent("Settings Key"));
    }
}
