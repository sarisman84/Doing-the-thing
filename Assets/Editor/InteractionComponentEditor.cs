using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactivity;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(InteractionComponent))]
    public class InteractionComponentEditor : UnityEditor.Editor
    {
        public enum ArgumentType
        {
            String,
            Object
        }

        private InteractionComponent _interactionComponent;
        private bool _useEventManager;

        private SerializedProperty _pEventName;
        private SerializedProperty _pEventArgs;
        private SerializedProperty _pEventStringArgs;
        private SerializedProperty _pEventManagerUse;
        private SerializedProperty _pInteractionType;
        private SerializedProperty _pDamageEvent;

        private List<SerializedProperty> _pMainVariables = new List<SerializedProperty>();
        private ReorderableList _list;
        private Dictionary<int, Type> _typeList;
        private ArgumentType _type;

        private void OnEnable()
        {
            _pEventName = serializedObject.FindProperty("eventManagerEventName");
            _pEventArgs = serializedObject.FindProperty("eventManagerEventArgs");
            _pEventStringArgs = serializedObject.FindProperty("eventManagerEventStringArgs");
            _pEventManagerUse = serializedObject.FindProperty("useEventSystem");
            _pInteractionType = serializedObject.FindProperty("interactionType");
            _pDamageEvent = serializedObject.FindProperty("onDamageEvent");

            _pMainVariables.Add(serializedObject.FindProperty("needsToBeLookedAtForInteractivity"));
            _pMainVariables.Add(serializedObject.FindProperty("inputType"));
            _pMainVariables.Add(serializedObject.FindProperty("onInteractionEvent"));
            _pMainVariables.Add(serializedObject.FindProperty("onProximityEnterEvent"));
            _pMainVariables.Add(serializedObject.FindProperty("onProximityExitEvent"));
            _pMainVariables.Add(serializedObject.FindProperty("onAwakeEvent"));

            _typeList = new Dictionary<int, Type>();
            _list = new ReorderableList(serializedObject, _pEventArgs, true, true, true, true);
            _list.drawElementCallback += DrawElement;
            _list.drawHeaderCallback += DrawHeader;
        }


        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Event Arguments"));
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Vector2 originalSize = rect.size;
            Vector2 originalCenter = rect.center;
            EditorGUI.BeginProperty(rect, new GUIContent(), _pEventArgs.GetArrayElementAtIndex(index));
            rect.size -= new Vector2(240, 0);
            _type = (ArgumentType) EditorGUI.EnumPopup(rect, _type);

            rect.size = originalSize;
            switch (_type)
            {
                case ArgumentType.String:

                    SerializedProperty property = null;
                    if (_pEventStringArgs != null)
                    {
                        property = _pEventStringArgs.GetArrayElementAtIndex(index);
                        if (property == null)
                        {
                            _pEventStringArgs.InsertArrayElementAtIndex(index);
                            property = _pEventStringArgs.GetArrayElementAtIndex(index);
                        }
                    }

                    if (property == null)
                    {
                        EditorGUI.EndProperty();
                        return;
                    }

                    rect.center += new Vector2(140, 0);
                    rect.size -= new Vector2(140, 0);
                    property.stringValue = EditorGUI.TextField(rect, property.stringValue);
                    break;
                case ArgumentType.Object:
                    rect.center += new Vector2(140, 0);
                    rect.size -= new Vector2(140, 0);
                    EditorGUI.PropertyField(rect, _pEventArgs.GetArrayElementAtIndex(index), new GUIContent());
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override void OnInspectorGUI()
        {
            _interactionComponent = _interactionComponent ? _interactionComponent : target as InteractionComponent;

            EditorGUILayout.PropertyField(_pInteractionType);

            switch (_pInteractionType.enumValueIndex)
            {
                case 0:
                    DrawInteractionEventVariables();
                    break;

                case 1:
                    EditorGUILayout.PropertyField(_pDamageEvent);
                    break;

                case 2:
                    DrawInteractionEventVariables();
                    EditorGUILayout.PropertyField(_pDamageEvent);
                    break;
            }

            EditorGUILayout.PropertyField(_pMainVariables.Last());
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInteractionEventVariables()
        {
            EditorGUILayout.PropertyField(_pEventManagerUse);
            if (_pEventManagerUse.boolValue)
            {
                EditorGUILayout.PropertyField(_pEventName, new GUIContent(""));


                if (_pEventArgs == null) return;

                _list.DoLayoutList();
            }

            _pMainVariables.ApplyAction((s, index) =>
            {
                if (index == 2 && _pEventManagerUse.boolValue || index == _pMainVariables.Count - 1)
                    return;
                EditorGUILayout.PropertyField(s);
            });
        }
    }
}