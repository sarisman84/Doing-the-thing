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
            switch ((DetectionArea.ZoneTriggerType)_triggerType.enumValueIndex)
            {
                case DetectionArea.ZoneTriggerType.OnEnter:
                    EditorGUILayout.PropertyField(_onZoneEnter, new GUIContent("On Entering the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.OnExit:
                    EditorGUILayout.PropertyField(_onZoneExit, new GUIContent("On Exiting the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.OnStay:
                    EditorGUILayout.PropertyField(_onZoneStay, new GUIContent("While Staying in the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.OnEnterOrStay:
                    EditorGUILayout.PropertyField(_onZoneEnter, new GUIContent("On Entering the Zone"));
                    EditorGUILayout.PropertyField(_onZoneStay, new GUIContent("While Staying in the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.OnStayOrExit:
                    EditorGUILayout.PropertyField(_onZoneStay, new GUIContent("While Staying in the Zone"));
                    EditorGUILayout.PropertyField(_onZoneExit, new GUIContent("On Exiting the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.OnExitOrEnter:
                    EditorGUILayout.PropertyField(_onZoneEnter, new GUIContent("On Entering the Zone"));
                    EditorGUILayout.PropertyField(_onZoneExit, new GUIContent("On Exiting the Zone"));
                    break;
                case DetectionArea.ZoneTriggerType.All:
                    EditorGUILayout.PropertyField(_onZoneEnter, new GUIContent("On Entering the Zone"));
                    EditorGUILayout.PropertyField(_onZoneStay, new GUIContent("While Staying in the Zone"));
                    EditorGUILayout.PropertyField(_onZoneExit, new GUIContent("On Exiting the Zone"));
                    break;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}