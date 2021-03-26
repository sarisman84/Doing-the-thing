using System;
using System.Collections.Generic;
using Editor.PropertyDrawers;
using TPUModelerEditor;
using UnityEditor;
using UnityEngine;

namespace Editor.Extension
{
    public static class EditorUtilities
    {
        public enum ElementType
        {
            UnityEvent,
            UltEvent,
            Field,
            Array
        }

        public static float GetVisibleChildrenHeight(this SerializedProperty property)
        {
            float height = 0;
            property = property.Copy();
            while (property.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(property);
            }

            return height;
        }
        
        
        

        public static float GetPropertyHeight(SerializedProperty property = null, ElementType type = ElementType.Field)
        {
            int arraySize = 0;

            switch (type)
            {
                case ElementType.UnityEvent:
                    if (property == null) return 0;
                    arraySize = property.FindPropertyRelative("_PersistentCalls").arraySize;
                    arraySize = Mathf.Clamp(arraySize, 1, arraySize);
                    return
                        (FieldHeight + 20f) + (47 * arraySize) + 80f;

                case ElementType.UltEvent:
                    if (property == null) return 0;
                    arraySize = property.FindPropertyRelative("_PersistentCalls").arraySize;
                    arraySize = Mathf.Clamp(arraySize, 1, arraySize);
                    return
                        (((FieldHeight * 2f) * 1.15f) + (FieldHeight * 2f) * arraySize) + 50f;

                case ElementType.Field:
                    return FieldHeight;

                case ElementType.Array:
                    if (property == null) return 0;
                    arraySize = property.arraySize;
                    arraySize = Mathf.Clamp(arraySize, 1, arraySize);
                    return (FieldHeight + 20f) + ((FieldHeight + 5f) * arraySize) + 20f;
            }

            return 0;
        }

        static float FieldHeight => EditorGUIUtility.singleLineHeight;

        public static Rect NewElementPosition(this Rect position, ref Rect previousPosition, ElementType type,
            SerializedProperty property = null)
        {
            switch (type)
            {
                case ElementType.Field:
                    position.Set(previousPosition.x, previousPosition.y + FieldHeight + 5f,
                        previousPosition.width, FieldHeight);

                    break;
                case ElementType.UnityEvent:
                case ElementType.UltEvent:
                    position.Set(previousPosition.x,
                        previousPosition.y + previousPosition.height + 5f,
                        previousPosition.width,
                        GetPropertyHeight(property, type));
                    break;
                case ElementType.Array:
                    position.Set(previousPosition.x, previousPosition.y + previousPosition.height + 5f,
                        previousPosition.width, GetPropertyHeight(property, type));

                    break;
            }

            previousPosition = position;
            return position;
        }

        public static Rect UpdateCurrentPosition(this Rect position, ref Rect previousPosition, ElementType type,
            SerializedProperty property = null)
        {
            switch (type)
            {
                case ElementType.Field:
                    position.Set(position.x, position.y, position.width, FieldHeight);
                    break;
                case ElementType.UnityEvent:
                case ElementType.UltEvent:
                    position.Set(position.x, position.y, position.width,
                        GetPropertyHeight(property, type));
                    break;
                case ElementType.Array:
                    position.Set(position.x, position.y + FieldHeight, position.width,
                        GetPropertyHeight(property, type));
                    break;
            }

            previousPosition = position;
            return position;
        }

        public static Dictionary<string, SerializedProperty> FindProperties(SerializedProperty mainProperty,
            params string[] properties)
        {
            Dictionary<string, SerializedProperty> foundProperties = new Dictionary<string, SerializedProperty>();
            foreach (var property in properties)
            {
                if (foundProperties.ContainsKey(property))
                {
                    foundProperties[property] = mainProperty.FindPropertyRelative(property);
                    continue;
                }

                foundProperties.Add(property, mainProperty.FindPropertyRelative(property));
            }

            return foundProperties;
        }


        public static bool ToggleButton(Rect position, ref Rect previousPosition, SerializedProperty property,
            GUIContent label, GUIStyle style = null)
        {
            position = position.NewElementPosition(ref previousPosition, ElementType.Field);
            property.boolValue = EditorGUI.Toggle(position, property.boolValue, style ?? GUIStyles.defaultButtonStyle);
            position.Set(position.x + position.width / 2f, position.y, position.width,
                position.height);
            EditorGUI.LabelField(position, label);
            return property.boolValue;
        }

        public static Rect PropertyField(Rect position, ref Rect previousPosition,
            SerializedProperty property, ElementType type, GUIContent label = null, bool updatePos = false)
        {
            
            position = updatePos ? position.UpdateCurrentPosition(ref previousPosition, type) : position.NewElementPosition(ref previousPosition, type, property);
            EditorGUI.PropertyField(position, property,
                label ?? new GUIContent(property.displayName));

            return position;
        }

        public static string TooltipBuilder(bool condition, string message1, string message2 = "")
        {
            return condition
                ? message1
                : message2;
        }

        public static string TooltipBuilder(this string ogMessage, bool condition, string message)
        {
            return condition ? message : ogMessage;
        }
    }
}