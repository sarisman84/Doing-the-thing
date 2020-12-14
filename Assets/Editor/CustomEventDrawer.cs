using System;
using Interactivity.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EventListener))]
    public class CustomEventDrawer : UnityEditor.Editor
    {
        private SerializedProperty _eventsProperty;
        private ReorderableList _list;

        private void OnEnable()
        {
            _eventsProperty = serializedObject.FindProperty("events");
            _list = new ReorderableList(serializedObject, _eventsProperty, true, true, true, true);
            _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, _eventsProperty.displayName);
            _list.drawElementCallback = DrawElementCallback;
            _list.elementHeightCallback = ElementHeightCallback;
        }

        private float ElementHeightCallback(int index)
        {
            float height = EditorGUIUtility.singleLineHeight * 1.15f;
            SerializedProperty property = _list.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty unityResponse = property.FindPropertyRelative("unityResponse");
            if (property.isExpanded)
            {
                if (unityResponse.FindPropertyRelative("useExtraArgs").boolValue &&
                    (UnityEvents.EventType) unityResponse.FindPropertyRelative("eventType").enumValueIndex ==
                    UnityEvents.EventType.WeaponLibrary)
                    height += EditorGUIUtility.singleLineHeight * 20f;
                else
                    height += EditorGUIUtility.singleLineHeight * 11f;
            }

            return height;
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty listener =
                _eventsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("listener");
            Vector2 pos = rect.position;
            Vector2 size = rect.size;
            pos.x += 10f;
            size.x -= 10f;
            rect.position = pos;
            rect.size = size;
            EditorGUI.PropertyField(rect, _eventsProperty.GetArrayElementAtIndex(index),
                new GUIContent(listener.objectReferenceValue == null
                    ? _eventsProperty.GetArrayElementAtIndex(index).displayName
                    : listener.objectReferenceValue.name), true);
        }

        public override void OnInspectorGUI()
        {
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}