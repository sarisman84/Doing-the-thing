using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EventListAttribute))]
    public class EventListPropertyDrawer : PropertyDrawer
    {
        private float _height = 0;
        private int index = 0;
        private float offset = 50f;

        public static List<string> eventNames = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            _height = position.height;
            if (eventNames.Count != 0)
            {
                index = EditorGUI.Popup(position, index, eventNames.ToArray());
                position.y += offset;
            }

            Vector2 size = position.size;
            size.x /= 2f;
            position.size = size;
            string result = string.Empty;
            Vector3 newPos = position.position + new Vector2(position.size.x + 5f, 0);
            result = EditorGUI.TextField(position, result);
            position.position = newPos;
            if (GUI.Button(position, "Add Event"))
            {
                eventNames.Add(result);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float finalOffset = offset / 2f;
            return base.GetPropertyHeight(property, label) + finalOffset;
        }
    }
}