using System;
using Interactivity.Components.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DoorController))]
    public class DoorControllerEditor : UnityEditor.Editor
    {
        private DoorController _selfRef;

        private void OnEnable()
        {
            _selfRef = target as DoorController;
        }


        private void OnSceneGUI()
        {
            Handles.color = Color.green;
            var position = _selfRef.transform.position;
            Handles.ConeHandleCap(0, position,
                Quaternion.LookRotation(((_selfRef.DoorOpenPosition + position) - position).normalized), 0.25f,
                EventType.Repaint);

            Handles.color = Color.white;
            Handles.DrawLine(position, _selfRef.DoorOpenPosition + position);

            Handles.color = Color.red;
            Handles.CubeHandleCap(0, _selfRef.DoorOpenPosition + position,
                _selfRef.transform.rotation, _selfRef.transform.localScale.x, EventType.Repaint);

            // if (_selfRef.doorOpenPosition == Vector3.zero)
            //     _selfRef.doorOpenPosition += _selfRef.transform.forward;
            _selfRef.doorOpenPosition =
                Handles.PositionHandle(_selfRef.DoorOpenPosition + position, Quaternion.identity) - position;


            Handles.color = Color.red - new Color(0, 0, 0, 0.7f);
            Handles.DrawWireCube(_selfRef.DoorOpenPosition + position,
                _selfRef.transform.localScale);

            EditorUtility.SetDirty(target);
        }
    }
}