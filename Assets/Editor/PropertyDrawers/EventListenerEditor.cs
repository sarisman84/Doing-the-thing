using System;
using System.Collections.Generic;
using Interactivity.Events;
using Interactivity.Events.Listener;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Event = Interactivity.Events.Listener.Event;

namespace Editor.PropertyDrawers
{
    [CustomEditor(typeof(NewEventListener))]
    public class EventListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty _eventList;
        private SerializedProperty firstItem;
        private ReorderableList _list;
        public static Dictionary<SerializedObject, int> ReorderableListCount = new Dictionary<SerializedObject, int>();


        private void OnEnable()
        {
            _eventList = serializedObject.FindProperty("events");
            _list = new ReorderableList(serializedObject, _eventList, true, true, true, true);
            _list.drawElementCallback = DrawElementCallback;
            _list.drawHeaderCallback = DrawHeaderCallback;
            _list.elementHeightCallback = ElementHeightCallback;
            ReorderableListCount.Add(serializedObject, _list.count);
        }

        private float ElementHeightCallback(int index)
        {
            SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, element.isExpanded);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, _eventList.displayName);
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, new GUIContent(element.displayName), element.isExpanded);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            _list.DoLayoutList();
            ReorderableListCount[serializedObject] = _list.count;
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomPropertyDrawer(typeof(Event))]
    public class EventPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();
        private float _spacing = 5f;
        float totalHeight = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty eventToListenTo, conditions, responseToEvent;
            Rect r = position;

            eventToListenTo = property.FindPropertyRelative("eventToListenTo");
            conditions = property.FindPropertyRelative("conditions");
            responseToEvent = property.FindPropertyRelative("responseToEvent");

            EditorGUI.BeginChangeCheck();

            GUI.Box(new Rect(position.x, position.y, position.width, totalHeight), GUIContent.none);
            EditorGUI.indentLevel++;
            DrawPropertyField(ref r, eventToListenTo);
            if (eventToListenTo.objectReferenceValue != null &&
                eventToListenTo.objectReferenceValue is CustomEvent customEvent && !customEvent.isEventGlobal)
            {
                DrawReorderableList(ref r, conditions);
            }

            DrawPropertyField(ref r, responseToEvent, false);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawReorderableList(ref Rect rect, SerializedProperty property)
        {
            if (!_reorderableLists.ContainsKey(property.propertyPath))
            {
                ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);
                list.drawElementCallback = (rect1, index, active, focused) =>
                    DrawElementCallback(rect1, property, index, active, focused);
                list.drawHeaderCallback = DrawHeaderCallback;
                list.elementHeightCallback = (index) => ElementHeightCallback(index, property);
                _reorderableLists.Add(property.propertyPath, list);
            }

            SerializedProperty it = GetFirstSerializedProperty(property);
            it.Next(true);
            rect.height = SingleListHeight(property, it) + 25f;
            _reorderableLists[property.propertyPath].DoList(rect);
            rect.y += rect.height + _spacing;
        }

        private static float SingleListHeight(SerializedProperty property, SerializedProperty it)
        {
            return (EditorGUI.GetPropertyHeight(it, it.isExpanded) + (EditorGUIUtility.singleLineHeight * 1.15f) + (
                       EventListenerEditor.ReorderableListCount.ContainsKey(property.serializedObject) &&
                       EventListenerEditor.ReorderableListCount[property.serializedObject] > 1
                           ? 20f
                           : 0)) *
                   Mathf.Min(1f, it.Copy().CountRemaining());
        }

        private float ElementHeightCallback(int index, SerializedProperty property)
        {
            SerializedProperty element = _reorderableLists[property.propertyPath].serializedProperty
                .GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, element.isExpanded);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Current Conditions:");
        }

        private void DrawElementCallback(Rect rect, SerializedProperty property, int index, bool isactive,
            bool isfocused)
        {
            Rect r = rect;
            DrawPropertyField(ref r,
                _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index));
        }

        private void DrawPropertyField(ref Rect r, SerializedProperty property, bool editRect = true)
        {
            r.height = EditorGUI.GetPropertyHeight(property, property.isExpanded);
            EditorGUI.PropertyField(r, property, new GUIContent(property.displayName));
            if (editRect)

                r.y += r.height + _spacing;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            totalHeight = 0;
            var it = GetFirstSerializedProperty(property);
            SerializedProperty arrayP = default;
            it.Next(true);
            while (it.NextVisible(true))
            {
                if (arrayP != null && it.propertyPath.Contains(arrayP.propertyPath))
                {
                    continue;
                }

                if (it.propertyType == SerializedPropertyType.Generic)
                {
                    arrayP = it;
                }

                totalHeight +=  (SingleListHeight(property, it) + 25f);
            }

            totalHeight -= 120f;

            return base.GetPropertyHeight(property, label) + totalHeight;
        }

        private static SerializedProperty GetFirstSerializedProperty(SerializedProperty property)
        {
            SerializedProperty it = property.serializedObject.GetIterator();
            return it;
        }
    }
}