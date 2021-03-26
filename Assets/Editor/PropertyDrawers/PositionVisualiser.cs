using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    //Taken by "Propertydrawer": https://gist.github.com/soraphis/df2ca801c94fa95e0c940b40b3655284
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class PositionHandleEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneUpdate;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneUpdate;
        }

        void OnSceneUpdate(SceneView view)
        {
            if (Application.isPlaying) return;
            var t = target as MonoBehaviour;
            if (!t) return;

            foreach (var fieldInfo in t.GetType().GetFields())
            {
                try
                {
                    var attribs = fieldInfo.GetCustomAttributes(typeof(VisualisePosition), false);
                    if (attribs.Length > 0)
                    {
                        foreach (var attrib in attribs)
                        {
                            VisualisePosition visualisePosition = attrib as VisualisePosition;
                            if (fieldInfo.FieldType == typeof(Vector3))
                            {
                                Vector3 v = (Vector3) fieldInfo.GetValue(t);
                                v = DrawPositionHandle(t, visualisePosition, v, fieldInfo.Name);
                                fieldInfo.SetValue(t, v);
                            }
                            else if (fieldInfo.FieldType == typeof(List<Vector3>))
                            {
                                List<Vector3> l = (List<Vector3>) fieldInfo.GetValue(t);
                                if (l != null)
                                    for (int i = 0; i < l.Count; i++)
                                    {
                                        l[i] = DrawPositionHandle(t, visualisePosition, l[i],
                                            $"{fieldInfo.Name}({i})");
                                    }

                                fieldInfo.SetValue(t, l);
                            }
                            else if (fieldInfo.FieldType == typeof(Vector3[]))
                            {
                                Vector3[] l = (Vector3[]) fieldInfo.GetValue(t);
                                if (l != null)
                                    for (int i = 0; i < l.Length; i++)
                                    {
                                        l[i] = DrawPositionHandle(t, visualisePosition, l[i],
                                            $"{fieldInfo.Name}({i})");
                                    }

                                fieldInfo.SetValue(t, l);
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    // ignored
                }
            }
        }

        private Vector3 DrawPositionHandle(MonoBehaviour t, VisualisePosition visualisePosition, Vector3 v,
            string label)
        {
            if (visualisePosition.CanMoveHandle && !Application.isPlaying)
            {
                Vector3 offsetV = Handles.PositionHandle(
                    v + (visualisePosition.IsPositionLocalized ? t.transform.position : Vector3.zero),
                    Quaternion.identity);
                v = offsetV - ((visualisePosition.IsPositionLocalized)
                    ? t.transform.position
                    : Vector3.zero);
            }
            else if (!Application.isPlaying)
            {
                if (!Application.isPlaying)
                    _localPosition = v + t.transform.position;
                Handles.color = Color.gray - new Color(0, 0, 0, 0.75f);
                Handles.CubeHandleCap(GUIUtility.GetControlID(FocusType.Passive),
                    (visualisePosition.IsPositionLocalized ? _localPosition : v),
                    Quaternion.identity, 1,
                    EventType.Repaint);
                Handles.color = Color.cyan;
                Handles.SphereHandleCap(GUIUtility.GetControlID(FocusType.Passive),
                    (visualisePosition.IsPositionLocalized ? _localPosition : v),
                    Quaternion.identity, 0.1f,
                    EventType.Repaint);
            }

            if (visualisePosition.CanShowVariable)
                Handles.Label(v, label);
            return v;
        }

        private Vector3 _localPosition;
    }


    // [CustomPropertyDrawer(typeof(VisualisePosition))]
    // public class VisualizePositionDrawer : PropertyDrawer
    // {
    //     void OnSceneGUI(SceneView view)
    //     {
    //     }
    // }
}