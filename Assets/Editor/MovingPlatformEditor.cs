﻿using System;
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

        private void OnEnable()
        {
            SceneView.duringSceneGui += sv => OnSceneGUI();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= sv => OnSceneGUI();
        }

        private void OnSceneGUI()
        {
            DrawLinesBetweenWaypoints(((MovingPlatform) target).waypointList);
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