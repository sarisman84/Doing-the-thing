using System;
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
        

        private void OnEnable()
        {
      
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty triggerType = serializedObject.FindProperty("triggerType");
        
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoneType"));
            EditorGUILayout.PropertyField(triggerType);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoneColor"));
            EditorGUILayout.Space();
            SerializedProperty layer = serializedObject.FindProperty("detectionMask");
            EditorGUILayout.PropertyField(layer);



         
            
            
            EditorGUILayout.Space();
            switch (triggerType.enumValueIndex)
            {
                case 0:
                    SerializedProperty onEnterEvent = serializedObject.FindProperty("onZoneEnter");
                    EditorGUILayout.PropertyField(onEnterEvent, new GUIContent("On Entering the Zone"));
                    break;
                case 1:
                    SerializedProperty onExitEvent = serializedObject.FindProperty("onZoneExit");
                    EditorGUILayout.PropertyField(onExitEvent, new GUIContent("On Exiting the Zone"));
                    break;
                case 2:
                    SerializedProperty onStayEvent = serializedObject.FindProperty("onZoneExit");
                    EditorGUILayout.PropertyField(onStayEvent, new GUIContent("On Staying in the Zone"));
                    break;
                
                default:
                    Debug.Log(triggerType.enumValueIndex.ToString());
                    break;
            }


            serializedObject.ApplyModifiedProperties();
        }

      
    }
}