using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RestrictToType))]
public class RestrictToTypePropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType == SerializedPropertyType.ObjectReference) {
            var requiredAttribute = this.attribute as RestrictToType;
            EditorGUI.BeginProperty(position, label, property);
            Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);
            if(obj is GameObject g) {
                property.objectReferenceValue = g.GetComponent(requiredAttribute.requiredType);
            }
            EditorGUI.EndProperty();
        } else {
            var previousColor = GUI.color;
            GUI.color = Color.red;
            EditorGUI.LabelField(position, label, new GUIContent("Property is not the correct type"));
            GUI.color = previousColor;
        }
    }
}