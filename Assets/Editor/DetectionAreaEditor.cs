using System;
using Interactivity.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Editor
{
    [CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(DetectionArea))]
    public class DetectionAreaEditor : UnityEditor.Editor
    {
        private SerializedProperty _onZoneEnter, _onZoneExit, _onZoneStay;
        private SerializedProperty _triggerType;
        private SerializedProperty _zoneColor, _zoneType, _detectionMask;

        private SerializedProperty _onZoneEnterCustom, _onZoneExitCustom, _onZoneStayCustom;

        private void OnEnable()
        {
            _zoneType = serializedObject.FindProperty("zoneType");
            _triggerType = serializedObject.FindProperty("triggerType");
            _zoneColor = serializedObject.FindProperty("zoneColor");
            _detectionMask = serializedObject.FindProperty("detectionMask");

            _onZoneEnter = serializedObject.FindProperty("onZoneEnter");


            _onZoneExit = serializedObject.FindProperty("onZoneExit");


            _onZoneStay = serializedObject.FindProperty("onZoneStay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.PropertyField(_zoneType);
            EditorGUILayout.PropertyField(_triggerType);
            EditorGUILayout.PropertyField(_zoneColor);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_detectionMask);


            EditorGUILayout.Space();
            switch (_triggerType.enumValueIndex)
            {
                case 0:

                    EditorGUILayout.PropertyField(_onZoneEnter, new GUIContent("On Entering the Zone"));

                    break;
                case 1:

                    EditorGUILayout.PropertyField(_onZoneExit, new GUIContent("On Exiting the Zone"));

                    break;
                case 2:
                    EditorGUILayout.PropertyField(_onZoneStay, new GUIContent("While Staying in the Zone"));

                    break;

                default:
                    Debug.Log(_triggerType.enumValueIndex.ToString());
                    break;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}