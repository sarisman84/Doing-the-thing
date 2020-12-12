using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(FancyListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{
    private ReorderableList _list;
    private SerializedObject _serializedObject;

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        DrawList(position, property, label);
        // EditorGUI.PropertyField(position, property, label);
        // Debug.Log(property.propertyPath);
    }

    private void DrawList(Rect position, SerializedProperty property, GUIContent label)
    {
        _serializedObject = _serializedObject ?? property.serializedObject;
        string[] path = property.propertyPath.Split(new[] {".Array.data"}, StringSplitOptions.RemoveEmptyEntries);
        if (property.propertyPath.Contains("Array"))
        {
            SerializedProperty foundProperty = _serializedObject.FindProperty(path[0]);
            _list = _list ?? new ReorderableList(_serializedObject, foundProperty, true, true,
                true, true);
            _list.drawElementCallback = (rect, index, active, focused) =>
                EditorGUI.PropertyField(rect, foundProperty.GetArrayElementAtIndex(index),
                    new GUIContent(foundProperty.GetArrayElementAtIndex(index).displayName));
            _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, foundProperty.displayName);
            _list.DoLayoutList();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}