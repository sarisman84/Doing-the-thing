using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Extension
{
    public class ExtendedEditor : UnityEditor.Editor
    {
        protected SerializedProperty currentProperty;
        protected string selectedPropertyPath;
        protected SerializedProperty selectedProperty;
        protected Dictionary<string, ReorderableList> drawnLists = new Dictionary<string, ReorderableList>();

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) continue;
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }


        protected void DrawList(SerializedProperty prop, int defaultListElementAmount = 1,
            Action<Rect, SerializedProperty> customDrawCall = null,
            Action<ReorderableList> customSelectCall = null)
        {
            if (prop == null) return;
            if (!drawnLists.ContainsKey(prop.name))
            {
                ReorderableList list = new ReorderableList(serializedObject, prop, true, true, true, true);
                list.onCanRemoveCallback = reorderableList => reorderableList.count > defaultListElementAmount;
                list.onChangedCallback = reorderableList => customSelectCall?.Invoke(reorderableList);
                list.onSelectCallback = reorderableList => { customSelectCall?.Invoke(reorderableList); };
                list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, prop.displayName);
                list.drawElementCallback = (rect, index, active, focused) =>
                {
                    SerializedProperty p = prop.GetArrayElementAtIndex(index);

                    if (customDrawCall != null)
                    {
                        customDrawCall.Invoke(rect, p);
                    }
                    else
                    {
                        if (GUI.Button(rect, p.displayName))
                        {
                            selectedPropertyPath = p.propertyPath;
                        }

                        if (!string.IsNullOrEmpty(selectedPropertyPath))
                        {
                            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
                        }
                    }
                };
                drawnLists.Add(prop.name, list);
            }

            drawnLists[prop.name].DoLayoutList();
        }

        protected void SelectProperty(string prop)
        {
            if (!string.IsNullOrEmpty(prop))
            {
                selectedProperty = serializedObject.FindProperty(prop);
            }
        }


        public SerializedProperty DrawElement(string path, bool relative, bool includeChildren,
            Action<SerializedProperty> customDrawCall = null)
        {
            SerializedProperty newProperty =
                relative ? selectedProperty.FindPropertyRelative(path) : serializedObject.FindProperty(path);
            if (customDrawCall != null)
            {
                customDrawCall.Invoke(newProperty);
                return newProperty;
            }

            if (newProperty == null) return newProperty;
            EditorGUILayout.PropertyField(newProperty, includeChildren);
            return newProperty;
        }

        protected void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public struct Path
    {
        public string propertyPath;
        public bool relative;

        public Path(string value, bool b)
        {
            propertyPath = value;
            relative = b;
        }
    }

    public static class ExtendedEditorDrawer
    {
        public static SerializedProperty DrawMultipleElements(this SerializedProperty prop, SerializedObject obj,
            Action<SerializedProperty> customDrawCall = null, params Path[] paths)
        {
            foreach (var path in paths)
            {
                SerializedProperty newProperty = path.relative
                    ? prop.FindPropertyRelative(path.propertyPath)
                    : obj.FindProperty(path.propertyPath);
                if (newProperty == null) continue; ;
                customDrawCall?.Invoke(newProperty);

                if (customDrawCall == null) EditorGUILayout.PropertyField(newProperty, true);
            }

            return prop;
        }
    }
}