using System;
using Interactivity.Events;
using UnityEditor;
using UnityEngine;
using static Editor.PropertyDrawers.EditorExtensions;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(UnityEvents))]
    public class UnityEventsPropertyDrawer : PropertyDrawer
    {
        private UnityEvents.EventType _curValue;
        private SerializedProperty _enumVal;

        private SerializedProperty
            _eventParameterless,
            _eventInt,
            _eventFloat,
            _eventString,
            _eventCollider,
            _eventGameObject,
            _eventColor,
            _eventWeaponLibrary,
            _eventExtraWeaponLibrary;

        private SerializedProperty
            _eventArgsInt,
            _eventArgsFloat,
            _eventArgsString,
            _eventArgsCollider,
            _eventArgsGameObject,
            _eventArgsColor,
            _eventArgsWeaponLibrary,
            _eventArgsExtraWeaponLibrary;

        private SerializedProperty _useExtraArgs;

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            _enumVal = property.FindPropertyRelative("eventType");

            _eventParameterless = property.FindPropertyRelative("parameterlessEvent");
            _eventInt = property.FindPropertyRelative("intEvent");
            _eventFloat = property.FindPropertyRelative("floatEvent");
            _eventString = property.FindPropertyRelative("stringEvent");
            _eventCollider = property.FindPropertyRelative("colliderEvent");
            _eventGameObject = property.FindPropertyRelative("gameObjectEvent");
            _eventColor = property.FindPropertyRelative("colorEvent");
            _eventWeaponLibrary = property.FindPropertyRelative("weaponLibraryEvent");
            _eventExtraWeaponLibrary = property.FindPropertyRelative("extraWeaponLibraryEvent");

            _eventArgsInt = property.FindPropertyRelative("intArg");
            _eventArgsFloat = property.FindPropertyRelative("floatArg");
            _eventArgsString = property.FindPropertyRelative("stringArg");
            _eventArgsCollider = property.FindPropertyRelative("colliderArg");
            _eventArgsGameObject = property.FindPropertyRelative("gameObjectArg");
            _eventArgsColor = property.FindPropertyRelative("colorArg");
            _eventArgsWeaponLibrary = property.FindPropertyRelative("weaponLibraryArg");
            _eventArgsExtraWeaponLibrary = property.FindPropertyRelative("DelegateArg");

            _useExtraArgs = property.FindPropertyRelative("useExtraArgs");

            Vector2 size = position.size;
            size = new Vector2(size.x, size.y - EditorGUIUtility.singleLineHeight * 2.5f * property.CountRemaining());
            position.size = size;


            _curValue = (UnityEvents.EventType) EditorGUI.EnumPopup(position,
                (UnityEvents.EventType) _enumVal.enumValueIndex);
            _enumVal.enumValueIndex = (int) _curValue;


            EditPosition(ref position, 1.15f, RectAxis.Y);


            switch (_curValue)
            {
                case UnityEvents.EventType.Int:
                    DrawProperties(position, _eventInt, _eventArgsInt);
                    break;
                case UnityEvents.EventType.Float:
                    DrawProperties(position, _eventFloat, _eventArgsFloat);
                    break;
                case UnityEvents.EventType.String:
                    DrawProperties(position, _eventString, _eventArgsString);
                    break;
                case UnityEvents.EventType.Color:
                    DrawProperties(position, _eventColor, _eventArgsColor);
                    break;
                case UnityEvents.EventType.Collider:
                    DrawProperties(position, _eventCollider, _eventArgsCollider);
                    break;
                case UnityEvents.EventType.GameObject:
                    DrawProperties(position, _eventGameObject, _eventArgsGameObject);
                    break;
                case UnityEvents.EventType.Void:
                    DrawProperties(position, _eventParameterless, null);
                    break;
                case UnityEvents.EventType.WeaponLibrary:
                    EditorGUI.PropertyField(position, _useExtraArgs, new GUIContent(_useExtraArgs.displayName));
                    EditPosition(ref position, 1.15f, RectAxis.Y);
                    if (_useExtraArgs.boolValue)
                    {
                        DrawProperties(position, _eventArgsWeaponLibrary);
                        
                        EditPosition(ref position, 1.15f, RectAxis.Y);
                        
                        DrawProperties(position, _eventArgsExtraWeaponLibrary);

                        EditPosition(ref position, 6.25f, RectAxis.Y);

                        DrawProperties(position, _eventExtraWeaponLibrary);
                    }
                    else
                        DrawProperties(position, _eventWeaponLibrary, _eventArgsWeaponLibrary);

                    break;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            height += EditorGUIUtility.singleLineHeight * 2.5f * property.CountRemaining();

            return height;
        }

        void DrawProperties(Rect position, SerializedProperty @event, params SerializedProperty[] arguments)
        {
            if (arguments == null && @event != null)
            {
                EditorGUI.PropertyField(position, @event, new GUIContent(@event.displayName), true);
                return;
            }

            Vector2 ogSize = position.size;
            if (arguments != null)
                foreach (SerializedProperty argument in arguments)
                {
                    if (argument == null) continue;

                    EditPosition(ref position, 0.20f, RectAxis.Y);

                    EditorGUI.PropertyField(position, argument, new GUIContent(argument.displayName), true);
                }

            if (@event == null) return;
            EditPosition(ref position, 1.15f, RectAxis.Y);

            position.size = ogSize;
            EditorGUI.PropertyField(position, @event, new GUIContent(@event.displayName), true);
        }

        public void EditPosition(ref Rect value, float newPos, RectAxis axis, bool subtract = false)
        {
            Vector2 pos = value.position;
            switch (axis)
            {
                case RectAxis.X:

                    pos = subtract
                        ? new Vector2(pos.x - EditorGUIUtility.singleLineHeight * newPos, pos.y)
                        : new Vector2(pos.x + EditorGUIUtility.singleLineHeight * newPos, pos.y);
                    break;
                case RectAxis.Y:
                    pos = subtract
                        ? new Vector2(pos.x, pos.y - EditorGUIUtility.singleLineHeight * newPos)
                        : new Vector2(pos.x, pos.y + EditorGUIUtility.singleLineHeight * newPos);
                    break;
                case RectAxis.Both:
                    pos = subtract
                        ? new Vector2(pos.x - EditorGUIUtility.singleLineHeight * newPos,
                            pos.y - EditorGUIUtility.singleLineHeight * newPos)
                        : new Vector2(pos.x + EditorGUIUtility.singleLineHeight * newPos,
                            pos.y + EditorGUIUtility.singleLineHeight * newPos);
                    break;
            }

            value.position = pos;
        }
    }


    public static class EditorExtensions
    {
        public enum RectAxis
        {
            X,
            Y,
            Both
        }


        public static void EditPosition(this Rect value, Action<Vector2> calculation)
        {
            Vector2 pos = value.position;

            calculation?.Invoke(pos);

            value.position = pos;
        }
    }
}