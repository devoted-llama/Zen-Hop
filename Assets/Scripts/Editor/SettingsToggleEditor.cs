using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
[CustomEditor(typeof(SettingsToggle))]
public class SettingsToggleEditor : ToggleEditor {

    public override void OnInspectorGUI() {
        SettingsToggle component = (SettingsToggle)target;

        base.OnInspectorGUI();
        
        component.SettingsKey = EditorGUILayout.TextField("Settings Key", component.SettingsKey);

    }
}
