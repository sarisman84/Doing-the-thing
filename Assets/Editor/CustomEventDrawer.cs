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

            if (isNotNull && isInstanceEvent)
            {
                height += FieldHeight;
                if (useList)
                {
                    height += FieldHeight * (property.FindPropertyRelative("entityComparison").arraySize + 1);
                    height += GetUnityEventHeight(property.FindPropertyRelative("entityUnityEvent"));
                }

                else
                    height += GetUnityEventHeight(property.FindPropertyRelative("defaultUnityEvent"));
            }
            else
            {
                height += GetUnityEventHeight(property.FindPropertyRelative("defaultUnityEvent")) - 60f;
            }


            return height;
        }

        // private float ElementHeightCallback(int index)
        // {
        //     float height = EditorGUIUtility.singleLineHeight * 1.15f;
        //     SerializedProperty property = _list.serializedProperty.GetArrayElementAtIndex(index);
        //     SerializedProperty unityResponse = property.FindPropertyRelative("eventListener");
        //     SerializedProperty call = property
        //         .FindPropertyRelative("entityUnityEvent");
        //     SerializedProperty normalCall = property.FindPropertyRelative("defaultUnityEvent");
        //     SerializedProperty entityComparison = property.FindPropertyRelative("entityComparison");
        //     SerializedProperty useCustomArgs = property.FindPropertyRelative("useCustomArg");
        //
        //     if (unityResponse.objectReferenceValue != null && unityResponse.objectReferenceValue is InstanceEvent &&
        //         useCustomArgs.boolValue)
        //         height = FieldHeight * 2f +
        //                  (FieldHeight * entityComparison.arraySize + 1) * 4.75f +
        //                  GetUnityEventHeight(call) * 2f;
        //     else if (unityResponse.objectReferenceValue != null)
        //         height = (FieldHeight * 2f) + GetUnityEventHeight(normalCall) * (index != 0 ? 2f : 1);
        //
        //     return height;
        // }

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
        private const string EntityEvent = "entityUnityEvent";
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
            FindProperties(_foundProperties, property, ComparisonList, CompareEntities, UnityEvent, EntityEvent,
                EventListener);

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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsInstanceEvent
                ? _foundProperties[CompareEntities].boolValue
                    ? base.GetPropertyHeight(_foundProperties[EventListener], new GUIContent()) +
                      base.GetPropertyHeight(_foundProperties[CompareEntities], new GUIContent()) +
                      base.GetPropertyHeight(_foundProperties[ComparisonList], new GUIContent()) +
                      base.GetPropertyHeight(_foundProperties[EntityEvent], new GUIContent())
                    : base.GetPropertyHeight(_foundProperties[EventListener], new GUIContent()) +
                      base.GetPropertyHeight(_foundProperties[CompareEntities], new GUIContent()) +
                      base.GetPropertyHeight(_foundProperties[UnityEvent], new GUIContent())
                : base.GetPropertyHeight(_foundProperties[EventListener], new GUIContent()) +
                  base.GetPropertyHeight(_foundProperties[UnityEvent], new GUIContent());
        }

        public enum ElementType
        {
            Field,
            UnityEvent,
            List
        }

        private void NextElementPosition(ref Rect position, ElementType type)
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
                        GetUnityEventHeight(IsInstanceEvent
                            ? _foundProperties[EntityEvent]
                            : _foundProperties[UnityEvent]));
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
            int arraySize = property.FindPropertyRelative("m_PersistentCalls")
                .FindPropertyRelative("m_Calls").arraySize;
            return
                (FieldHeight + 80f) + (47f * (arraySize == 0 ? arraySize + 1 : arraySize)) + 20f;
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

                NextElementPosition(ref position, ElementType.UnityEvent);
                EditorGUI.PropertyField(position, foundProperties[EntityEvent],
                    new GUIContent(foundProperties[EntityEvent].displayName));
                return;
            }

            NextElementPosition(ref position, ElementType.UnityEvent);
            EditorGUI.PropertyField(position, foundProperties[UnityEvent],
                new GUIContent(foundProperties[UnityEvent].displayName));

            _serializedObject.ApplyModifiedProperties();


            //  SerializedProperty eventListener = property.FindPropertyRelative("eventListener");
            // gameObjectArg = property.FindPropertyRelative("entityComparison");
            //
            // SerializedProperty defaultUnityEvent = property.FindPropertyRelative("defaultUnityEvent");
            // SerializedProperty entityUnityEvent = property.FindPropertyRelative("entityUnityEvent");
            //
            // SerializedProperty useCustomArg = property.FindPropertyRelative("useCustomArg");
            //
            // position.Set(position.position.x, position.position.y, position.width, 20);
            // EditorGUI.PropertyField(position, eventListener, new GUIContent(eventListener.displayName));
            // position.Set(position.position.x, position.position.y + 25, position.width, position.height);
            // if (eventListener.objectReferenceValue is InstanceEvent)
            // {
            //     float ogXPos = position.position.x;
            //     float ogWidth = position.width;
            //
            //     position.Set(position.position.x, position.position.y, ogWidth,
            //         position.height);
            //     useCustomArg.boolValue = EditorGUI.Toggle(position,
            //         useCustomArg.boolValue, GUIStyles.helpButtonStyle);
            //     position.Set(position.position.x + 25, position.y, position.width, position.height);
            //     EditorGUI.LabelField(position, useCustomArg.boolValue ? "Disable comparison" : "Compare gameObjects");
            //     position.Set(ogXPos, position.position.y + EditorGUIUtility.singleLineHeight * 1.25f,
            //         ogWidth,
            //         position.height);
            //     if (useCustomArg.boolValue)
            //         DrawList(ref position, gameObjectArg);
            //     position.Set(ogXPos,
            //         useCustomArg.boolValue ? position.position.y + 75 : position.position.y + 25, ogWidth,
            //         position.height);
            //     EditorGUI.PropertyField(position, entityUnityEvent, new GUIContent(entityUnityEvent.displayName));
            //     return;
            // }
            //
            // if (eventListener.objectReferenceValue != null)
            //     EditorGUI.PropertyField(position, defaultUnityEvent, new GUIContent(defaultUnityEvent.displayName));
            //
            // property.serializedObject.ApplyModifiedProperties();
        }


        private void DrawHeaderCallback(Rect rect)
        {
            GUI.tooltip =
                "Toggle this if you want to compare the detected entity from the listener with another gameObject. This will make sure that the event is only called when both entities are the same.";
            EditorGUI.LabelField(rect, new GUIContent("Objects to compare"));
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty element = _currentLists[_currentProperty.displayName].serializedProperty
                .GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, new GUIContent(element.displayName));
        }
    }
}