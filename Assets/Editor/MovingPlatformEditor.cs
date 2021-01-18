using System;
using System.Collections.Generic;
using Interactivity.Moving_Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Editor
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MovingPlatform movingPlatform = target as MovingPlatform;


            if (!(movingPlatform is null) && movingPlatform.waypointList.Count == 0)
            {
                CreateWaypoint(movingPlatform);
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Add Waypoint"))
            {
                CreateWaypoint(movingPlatform);
            }

            if (GUILayout.Button("Remove Last Waypoint"))
            {
                if (!(movingPlatform is null))
                {
                    Vector3 waypoint = movingPlatform.waypointList[movingPlatform.waypointList.Count - 1];
                    movingPlatform.waypointList.Remove(waypoint);
                }
            }
        }

        private MovingPlatform _platform;

        private void OnEnable()
        {
            _platform = ((MovingPlatform) target);
            SceneView.duringSceneGui += sv => OnSceneGUI();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= sv => OnSceneGUI();
        }

        private void OnSceneGUI()
        {
            if (_platform)
                DrawLinesBetweenWaypoints(_platform.transform, ref _platform.waypointList);
        }

        private void CreateWaypoint(MovingPlatform _target)
        {
            if (_target == null) return;
            if (_target.waypointList == null)
                _target.waypointList = new List<Vector3>();
            _target.waypointList.Add(_target.waypointList.Count == 0
                ? _target.transform.position
                : _target.waypointList[_target.waypointList.Count - 1] +
                  Vector3.forward);
        }

        private void DrawLinesBetweenWaypoints(Transform transform, ref List<Vector3> targetWaypointList)
        {
            if (targetWaypointList == null) targetWaypointList = new List<Vector3>();
            Color originalColor = Color.blue;
            for (int i = 0; i < targetWaypointList.Count; i++)
            {
                Handles.color = originalColor;
                Vector3 waypointA = targetWaypointList[i];
                if (i == 0 || i == targetWaypointList.Count - 1)
                {
                    Handles.color = Color.yellow;
                    Handles.CubeHandleCap(0, waypointA, LookTowardsFutureWaypoint(i, targetWaypointList), 0.75f,
                        EventType.Repaint);
                }
                else
                {
                    Handles.color = Color.cyan;
                    Handles.ConeHandleCap(0, waypointA, LookTowardsFutureWaypoint(i, targetWaypointList), 1f,
                        EventType.Repaint);
                }

                if (i != 0)
                {
                    Handles.color = originalColor;
                    waypointA = Handles.PositionHandle(waypointA, Quaternion.identity);
                    targetWaypointList[i] = waypointA;
                }
                else
                {
                    Handles.color = originalColor;
                    if (!Application.isPlaying)
                        targetWaypointList[i] = transform.position;
                }

                if (i + 1 >= targetWaypointList.Count)
                {
                    Handles.color = Color.yellow;
                    Vector3 newWaypointB = targetWaypointList[0];
                    Handles.DrawDottedLine(waypointA, newWaypointB, 4f);
                    break;
                }

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