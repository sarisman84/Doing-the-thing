using System;
using System.Collections.Generic;
using Interactivity.Events;
using Interactivity.Events.Listener;
using TPUModelerEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Editor.EventDefinitionAttribute;

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
            float height = FieldHeight;

            SerializedProperty property = _list.serializedProperty.GetArrayElementAtIndex(index);
            bool isNotNull = property.FindPropertyRelative("eventListener").objectReferenceValue != null;
            bool isInstanceEvent = property.FindPropertyRelative("eventListener").objectReferenceValue is InstanceEvent;
            bool useList = property.FindPropertyRelative("useCustomArg").boolValue;
            if (!isNotNull) return height;
            if (isInstanceEvent)
            {
                height += FieldHeight;
                if (useList)
                {
                    height += FieldHeight * (property.FindPropertyRelative("entityComparison").arraySize + 1);
                    height += GetUnityEventHeight(property.FindPropertyRelative("instanceUnityEvent"));
                }

                else
                {
                    height += GetUnityEventHeight(property.FindPropertyRelative("instanceUnityEvent"));
                }
            }
            else
            {
                height += GetUnityEventHeight(property.FindPropertyRelative("defaultUnityEvent"));
            }


            return height;
        }


        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty listener =
                _eventsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("eventListener");
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


    [CustomPropertyDrawer(typeof(EventDefinition))]
    public class EventDefinitionAttribute : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _currentLists = new Dictionary<string, ReorderableList>();

        private Dictionary<string, SerializedProperty> _foundProperties;

        private SerializedObject _serializedObject;
        private SerializedProperty _currentProperty;

        private const string ComparisonList = "entityComparison";
        private const string CompareEntities = "useCustomArg";
        private const string UnityEvent = "defaultUnityEvent";
        private const string InstanceEvent = "instanceUnityEvent";
        private const string EventListener = "eventListener";

        private Rect _previousPosition;

        void FindProperties(Dictionary<string, SerializedProperty> foundProperties, SerializedProperty mainProperty,
            params string[] properties)
        {
            foreach (var property in properties)
            {
                if (foundProperties.ContainsKey(property))
                {
                    foundProperties[property] = mainProperty.FindPropertyRelative(property);
                    continue;
                }

                foundProperties.Add(property, mainProperty.FindPropertyRelative(property));
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _currentProperty = property;

            _serializedObject = property.serializedObject;
            _foundProperties = new Dictionary<string, SerializedProperty>();
            FindProperties(_foundProperties, property, ComparisonList, CompareEntities, UnityEvent,
                InstanceEvent, EventListener);

            if (!_currentLists.ContainsKey(property.displayName))
            {
                ReorderableList list = new ReorderableList(_serializedObject, _foundProperties[ComparisonList], true,
                    true, true,
                    true);
                list.drawElementCallback = DrawElementCallback;
                list.drawHeaderCallback = DrawHeaderCallback;
                _currentLists.Add(property.displayName, list);
            }


            EditorGUI.BeginProperty(position, label, property);
            OnInspectorGUI(_foundProperties, ref position, property);
            EditorGUI.EndProperty();
        }


        public enum ElementType
        {
            Field,
            UnityEvent,
            List
        }

        private void NextElementPosition(ref Rect position, ElementType type, string element = "")
        {
            switch (type)
            {
                case ElementType.Field:
                    position.Set(_previousPosition.x, _previousPosition.y + _previousPosition.height + 5f,
                        _previousPosition.width, FieldHeight);

                    break;
                case ElementType.UnityEvent:
                    position.Set(_previousPosition.x, _previousPosition.y + _previousPosition.height + 5f,
                        _previousPosition.width,
                        GetUnityEventHeight(_foundProperties[element]) / 2f);
                    break;
                case ElementType.List:
                    position.Set(_previousPosition.x, _previousPosition.y + _previousPosition.height + 5f,
                        _previousPosition.width, ListHeight);
                    break;
            }

            _previousPosition = position;
        }


        private void CurrentElementPosition(ref Rect position, ElementType type)
        {
            switch (type)
            {
                case ElementType.Field:
                    position.Set(position.x, position.y, position.width, FieldHeight);
                    break;
                case ElementType.UnityEvent:
                    position.Set(position.x, position.y, position.width,
                        FieldHeight * 2f);
                    break;
                case ElementType.List:
                    position.Set(position.x, position.y + FieldHeight, position.width, ListHeight * 2);
                    break;
            }

            _previousPosition = position;
        }


        public static float GetUnityEventHeight(SerializedProperty property)
        {
            if (property == null) return 0;
            int arraySize = property.FindPropertyRelative("_PersistentCalls").arraySize;
            arraySize = Mathf.Clamp(arraySize, 1, arraySize);
            return
                (FieldHeight + 20f) + (40 * arraySize) + 40f;
        }

        public static float FieldHeight => EditorGUIUtility.singleLineHeight * 1.15f;

        private float ListHeight
        {
            get
            {
                int arraySize = _currentLists[_currentProperty.displayName].count;
                return (FieldHeight) * (arraySize == 0 ? arraySize + 1 : arraySize) + FieldHeight * 2.15f;
            }
        }

        private bool IsInstanceEvent =>
            _foundProperties[EventListener].objectReferenceValue is InstanceEvent;


        private void OnInspectorGUI(Dictionary<string, SerializedProperty> foundProperties, ref Rect position,
            SerializedProperty property)
        {
            CurrentElementPosition(ref position, ElementType.Field);
            EditorGUI.PropertyField(position, foundProperties[EventListener],
                new GUIContent(
                    $"{(foundProperties[EventListener].objectReferenceValue == null ? "Insert event here" : $"When {foundProperties[EventListener].displayName} is called, do the following:")}"));
            if (foundProperties[EventListener].objectReferenceValue == null) return;
            if (foundProperties[EventListener].objectReferenceValue is InstanceEvent)
            {
                NextElementPosition(ref position, ElementType.Field);
                foundProperties[CompareEntities].boolValue = EditorGUI.Toggle(position,
                    new GUIContent(foundProperties[CompareEntities].boolValue
                        ? "Disable comparison"
                        : "Enable comparison"), foundProperties[CompareEntities].boolValue,
                    GUIStyles.bgPanelDarkStyle);

                if (foundProperties[CompareEntities].boolValue)
                {
                    NextElementPosition(ref position, ElementType.List);
                    _currentLists[property.displayName].DoList(position);
                }

                NextElementPosition(ref position, ElementType.UnityEvent, InstanceEvent);
                EditorGUI.PropertyField(position, foundProperties[InstanceEvent],
                    new GUIContent(foundProperties[InstanceEvent].displayName));
                return;
            }

            NextElementPosition(ref position, ElementType.UnityEvent, UnityEvent);
            EditorGUI.PropertyField(position, foundProperties[UnityEvent],
                new GUIContent(foundProperties[UnityEvent].displayName));
            _serializedObject.ApplyModifiedProperties();
        }


        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Objects to Compare"));
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty element = _currentLists[_currentProperty.displayName].serializedProperty
                .GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, new GUIContent(element.displayName));
        }
    }
}