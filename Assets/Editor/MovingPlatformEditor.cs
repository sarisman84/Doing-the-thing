using System;
using System.Collections.Generic;
using Interactivity.Moving_Objects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MovingPlatform _target = target as MovingPlatform;


            if (_target.waypointList.Count == 0)
            {
                CreateWaypoint(_target);
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Add Waypoint"))
            {
                CreateWaypoint(_target);
            }

            if (GUILayout.Button("Remove Last Waypoint"))
            {
                Vector3 waypoint = _target.waypointList[_target.waypointList.Count - 1];
                _target.waypointList.Remove(waypoint);
            }
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sv)
        {
            DrawLinesBetweenWaypoints(((MovingPlatform) target).waypointList);
        }

        private void CreateWaypoint(MovingPlatform _target)
        {
            _target.waypointList.Add(_target.waypointList[_target.waypointList.Count - 1] + Vector3.forward);
        }

        private void DrawLinesBetweenWaypoints(List<Vector3> targetWaypointList)
        {
            Color originalColor = Color.blue;
            for (int i = 0; i < targetWaypointList.Count; i++)
            {
                Handles.color = originalColor;
                Vector3 waypointA = targetWaypointList[i];
                if (i == 0 || i == targetWaypointList.Count - 1)
                {
                    Handles.color = Color.yellow;
                    Handles.SphereHandleCap(0, waypointA, LookTowardsFutureWaypoint(i, targetWaypointList), 2,
                        EventType.Repaint);
                }
                else
                {
                    Handles.color = Color.cyan;
                    Handles.ConeHandleCap(0, waypointA, LookTowardsFutureWaypoint(i, targetWaypointList), 1f,
                        EventType.Repaint);
                }

                Handles.color = originalColor;
                waypointA = Handles.PositionHandle(waypointA, Quaternion.identity);
                targetWaypointList[i] = waypointA;
                if (i + 1 >= targetWaypointList.Count) break;

                Vector3 waypointB = targetWaypointList[i + 1];

                Handles.DrawDottedLine(waypointA, waypointB, 4f);
            }
        }

        private Quaternion LookTowardsFutureWaypoint(int i, List<Vector3> targetWaypointList)
        {
            return i + 1 < targetWaypointList.Count
                ? Quaternion.LookRotation((targetWaypointList[i + 1] - targetWaypointList[i]))
                : Quaternion.identity;
        }
    }
}