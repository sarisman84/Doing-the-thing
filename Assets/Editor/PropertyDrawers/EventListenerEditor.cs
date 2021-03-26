using System;
using System.Collections.Generic;
using Editor.Extension;
using Editor.Scriptable_Wizards;
using Interactivity.Events;
using Interactivity.Events.Listener;
using TPUModelerEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Event = Interactivity.Events.Listener.Event;

namespace Editor.PropertyDrawers
{
    [CustomEditor(typeof(EventListener))]
    public class EventListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty eventProperty;
        private ReorderableList _reorderableList;


        private void OnEnable()
        {
            eventProperty = serializedObject.FindProperty("events");
            _reorderableList = new ReorderableList(serializedObject, eventProperty, true, true, true, true);
            _reorderableList.drawHeaderCallback += DrawHeaderCallback;
            _reorderableList.drawElementCallback += DrawElementCallback;
            _reorderableList.elementHeightCallback += ElementHeightCallback;
        }

        private float ElementHeightCallback(int index)
        {
            SerializedProperty prop = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop, prop.isExpanded);
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty prop = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop, new GUIContent(prop.displayName), prop.isExpanded);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Current Events");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Documentation"))
                EventListenerDocumentation.CreateWizard();

            EditorGUI.BeginChangeCheck();
            _reorderableList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        #region Old Code

        // private string _helpMessageTitle = "Using Event Listener 101:";
        //
        // private string _helpEventCreate =
        //     "To create an event, go to the Project window in Unity, right click and select: Create > Event > New Event then give said event a name. (Use the convention of (Object_Action). Example: Door_Open).";
        //
        // private string _helpUseEvent =
        //     "Once you create an event, you can use the event asset inside a Unity Event. Drag the event into the Unity Object Slot and select OnInvokeEvent. For normal, default Unity Events, select the one with the GameObject as an option. For custom Unity Events, select either the collider or the gameObject.";
        //
        // private string _helpListenToEvent =
        //     "To use this component, add an event listener by pressing the + button on the events list. This should create an element with a 'Event to Listen to', 'Response To Event()' and a 'Condition List'. Note that the Condition List appears when the event in 'Event To Listen to' is not a global event. To Remove an event listener, select the desired event listener to remove and the press the - on the list. This should remove the select element and automatically select the previous element in the list (if there is any).";
        //
        //
        // private bool eventCreateToggle;
        // private bool eventUseageToggle;
        // private bool listenerToEventToggle;
        // private SerializedProperty _eventList;
        // private SerializedProperty _firstItem;
        // private ReorderableList _list;
        // public static Dictionary<SerializedObject, int> ReorderableListCount = new Dictionary<SerializedObject, int>();
        //
        //
        // private void OnEnable()
        // {
        //     _eventList = serializedObject.FindProperty("events");
        //     _list = new ReorderableList(serializedObject, _eventList, true, true, true, true);
        //     _list.drawElementCallback = DrawElementCallback;
        //     _list.drawHeaderCallback = DrawHeaderCallback;
        //     _list.elementHeightCallback = ElementHeightCallback;
        //     ReorderableListCount.Add(serializedObject, _list.count);
        // }
        //
        // private float ElementHeightCallback(int index)
        // {
        //     SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
        //     return EditorGUI.GetPropertyHeight(element, element.isExpanded);
        // }
        //
        // private void DrawHeaderCallback(Rect rect)
        // {
        //     EditorGUI.LabelField(rect, _eventList.displayName);
        // }
        //
        // private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        // {
        //     SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
        //     EditorGUI.PropertyField(rect, element, new GUIContent(element.displayName), element.isExpanded);
        // }
        //
        // public override void OnInspectorGUI()
        // {
        //     HelpBox(_helpMessageTitle, MessageType.None);
        //
        //     if (GUILayout.Button(new GUIContent("How to create an event?")))
        //     {
        //         eventCreateToggle = !eventCreateToggle;
        //     }
        //
        //     if (eventCreateToggle)
        //         HelpBox(_helpEventCreate, MessageType.Info);
        //
        //     if (GUILayout.Button(new GUIContent("How to use an event?")))
        //     {
        //         eventUseageToggle = !eventUseageToggle;
        //     }
        //
        //     if (eventUseageToggle)
        //         HelpBox(_helpUseEvent, MessageType.Info);
        //
        //     if (GUILayout.Button(new GUIContent("How to use the event listener?")))
        //     {
        //         listenerToEventToggle = !listenerToEventToggle;
        //     }
        //
        //     if (listenerToEventToggle)
        //         HelpBox(_helpListenToEvent, MessageType.Info);
        //
        //     EditorGUI.BeginChangeCheck();
        //     _list.DoLayoutList();
        //     ReorderableListCount[serializedObject] = _list.count;
        //     if (EditorGUI.EndChangeCheck())
        //         serializedObject.ApplyModifiedProperties();
        // }
        //
        // private void HelpBox(string message, MessageType type)
        // {
        //     EditorGUILayout.HelpBox(message, type);
        // }

        #endregion
    }


    [CustomPropertyDrawer(typeof(Event))]
    public class EventPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();

        private Dictionary<string, Dictionary<string, SerializedProperty>> _registeredProperties =
            new Dictionary<string, Dictionary<string, SerializedProperty>>();

        private float _spacing = 5f;
        private float _registedPropertyHeight = 0;

        private const string EventToListenTo = "eventToListenTo",
            Conditions = "conditions",
            ResponseToEvent = "responseToEvent";


        #region Fetch Methods

        private SerializedProperty EventToListenToProperty(SerializedProperty property) =>
            _registeredProperties[property.propertyPath][EventToListenTo];

        private SerializedProperty ConditionsProperty(SerializedProperty property) =>
            _registeredProperties[property.propertyPath][Conditions];

        private SerializedProperty ResponseToEventProperty(SerializedProperty property) =>
            _registeredProperties[property.propertyPath][ResponseToEvent];

        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawViaSearch(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetPropertyHeightViaSearch(property, label);
        }

        private float GetPropertyHeightViaSearch(SerializedProperty property, GUIContent label)
        {
            _registedPropertyHeight = 0;
            float height = base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 1.5f;
            height += 5f;

            Dictionary<string, SerializedProperty> foundProperties = default;

            //Fetch the desired properties if a dictionary isn't created for this specific instance.
            if (!_registeredProperties.ContainsKey(property.propertyPath) ||
                _registeredProperties[property.propertyPath] == null)
            {
                foundProperties = new Dictionary<string, SerializedProperty>();
                foundProperties.Add(EventToListenTo, property.FindPropertyRelative(EventToListenTo));
                foundProperties.Add(Conditions, property.FindPropertyRelative(Conditions));
                foundProperties.Add(ResponseToEvent, property.FindPropertyRelative(ResponseToEvent));
            }

            //Assign the fetched properties to the dictionary by either creating and element or overriding it.
            if (!_registeredProperties.ContainsKey(property.propertyPath))
            {
                _registeredProperties.Add(property.propertyPath, foundProperties);
            }
            else if (_registeredProperties[property.propertyPath] == null)
            {
                _registeredProperties[property.propertyPath] = foundProperties;
            }


            //Increment the default property height of the current property to the resulting float.
            height += EditorGUI.GetPropertyHeight(EventToListenToProperty(property));

            //Workaround: As the default property height does not work for the second and onward elements of a list, manual calculation of the height works.
            float listHeight = (EditorGUIUtility.singleLineHeight * 2.5f) + EditorGUIUtility.singleLineHeight * 1.15f *
                Mathf.Clamp(ConditionsProperty(property).arraySize, 1, int.MaxValue);
            //For debug puproses and incase this gets fixed, there is the default property height to compare it to.
            float reportedHeight = EditorGUI.GetPropertyHeight(ConditionsProperty(property));

            //For now, lets increment the height by its manual calculation.
            height += listHeight;

            //Bellow debug log prints the current results of heights between the different methods.
            //Debug.Log(
            //    $"Property ({property.propertyPath}) contains {ConditionsProperty(property).name} with a size of {ConditionsProperty(property).arraySize}. Reported PropertyHeight is {reportedHeight} vs {listHeight}");


            //Increment the default property height of the current property to the resulting float.
            height += EditorGUI.GetPropertyHeight(ResponseToEventProperty(property));

            //Register the current height for later use.
            _registedPropertyHeight = height;
            return height;
        }

        private void DrawViaSearch(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            rect.y += 5f;
            SerializedProperty eventToListenTo, conditions, responseToEvent;

            eventToListenTo = EventToListenToProperty(property);
            conditions = ConditionsProperty(property);
            responseToEvent = ResponseToEventProperty(property);

            EditorGUI.BeginChangeCheck();

            //Draw a box to differentiate the different elements.
            GUI.Box(new Rect(position.x, position.y, position.width, _registedPropertyHeight - 5), GUIContent.none);
            EditorGUI.indentLevel++;
            DrawProperty(ref rect, eventToListenTo);

            //If an event exists and said event is not global, draw the list.
            if (eventToListenTo.objectReferenceValue != null &&
                eventToListenTo.objectReferenceValue is CustomEvent customEvent && !customEvent.isEventGlobal)
            {
                DrawReorderableList(ref rect, conditions);
            }

            DrawProperty(ref rect, responseToEvent);

            //If any changes are detected, apply them.
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws a PropertyField as well as correctly updates the rect position to allow for further elements to be drawn
        /// </summary>
        /// <param name="rect">The reference to the current position.</param>
        /// <param name="property">The property in question to draw.</param>
        private void DrawProperty(ref Rect rect, SerializedProperty property)
        {
            rect.height = EditorGUI.GetPropertyHeight(property, property.isExpanded);
            EditorGUI.PropertyField(rect, property, new GUIContent(property.displayName));
            rect.y += rect.height + _spacing;
        }

        /// <summary>
        /// Draws a ReorderableList using the assigned property. Said Reorderable List is saved in a dictionary so that multiple lists can be drawn. Similary to the DrawProperty method, the DrawReorderableList method updates the rect position to allow further elements to be drawn.
        /// </summary>
        /// <param name="rect">The reference to the current position.</param>
        /// <param name="property">The property in question to draw as a list.</param>
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


            rect.height = EditorGUIUtility.singleLineHeight * 2.5f;
            _reorderableLists[property.propertyPath].DoList(rect);
            rect.y += rect.height + _spacing + EditorGUIUtility.singleLineHeight * 1.15f *
                (Mathf.Clamp(property.arraySize, 1, int.MaxValue));
        }

        #region List Methods

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
            DrawProperty(ref r,
                _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index));
        }

        #endregion

        #region Old Code

        //     private Dictionary<string, ReorderableList> _reorderableLists = new Dictionary<string, ReorderableList>();
        //     private float _spacing = 5f;
        //     private float _totalHeight = 0;
        //     private float _extraHeight;
        //
        //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //     {
        //         SerializedProperty eventToListenTo, conditions, responseToEvent;
        //         Rect r = position;
        //         _extraHeight = 0;
        //         eventToListenTo = property.FindPropertyRelative("eventToListenTo");
        //         conditions = property.FindPropertyRelative("conditions");
        //         responseToEvent = property.FindPropertyRelative("responseToEvent");
        //
        //
        //         EditorGUI.BeginChangeCheck();
        //
        //         GUI.Box(new Rect(position.x, position.y, position.width, _totalHeight + _extraHeight), GUIContent.none);
        //         EditorGUI.indentLevel++;
        //         DrawPropertyField(ref r, eventToListenTo);
        //         if (eventToListenTo.objectReferenceValue != null &&
        //             eventToListenTo.objectReferenceValue is CustomEvent customEvent && !customEvent.isEventGlobal)
        //         {
        //             DrawReorderableList(ref r, conditions);
        //         }
        //
        //         DrawPropertyField(ref r, responseToEvent, false);
        //
        //         if (EditorGUI.EndChangeCheck())
        //         {
        //             property.serializedObject.ApplyModifiedProperties();
        //         }
        //
        //         EditorGUI.indentLevel--;
        //     }
        //
        //
        //     private void DrawReorderableList(ref Rect rect, SerializedProperty property)
        //     {
        //         if (!_reorderableLists.ContainsKey(property.propertyPath))
        //         {
        //             ReorderableList list = new ReorderableList(property.serializedObject, property, true, true, true, true);
        //             list.drawElementCallback = (rect1, index, active, focused) =>
        //                 DrawElementCallback(rect1, property, index, active, focused);
        //             list.drawHeaderCallback = DrawHeaderCallback;
        //             list.elementHeightCallback = (index) => ElementHeightCallback(index, property);
        //             _reorderableLists.Add(property.propertyPath, list);
        //         }
        //
        //         SerializedProperty it = GetFirstSerializedProperty(property);
        //         it.Next(true);
        //         rect.height = SingleListHeight(property, it) + 15f;
        //         _reorderableLists[property.propertyPath].DoList(rect);
        //         rect.y += rect.height + _spacing;
        //     }
        //
        //     private static float SingleListHeight(SerializedProperty property, SerializedProperty it)
        //     {
        //         int size = property.isArray && property.propertyType == SerializedPropertyType.Generic ? property.arraySize : 0;
        //         if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        //             Debug.Log(property.arraySize);
        //         return (EditorGUI.GetPropertyHeight(it, it.isExpanded) + (EditorGUIUtility.singleLineHeight * 1.15f) + (
        //                    EventListenerEditor.ReorderableListCount.ContainsKey(property.serializedObject) &&
        //                    EventListenerEditor.ReorderableListCount[property.serializedObject] > 1
        //                        ? 20f
        //                        : 0)) *
        //                Mathf.Min(1f, size + 1);
        //     }
        //
        //     private float ElementHeightCallback(int index, SerializedProperty property)
        //     {
        //         SerializedProperty element = _reorderableLists[property.propertyPath].serializedProperty
        //             .GetArrayElementAtIndex(index);
        //         return EditorGUI.GetPropertyHeight(element, element.isExpanded);
        //     }
        //
        //     private void DrawHeaderCallback(Rect rect)
        //     {
        //         EditorGUI.LabelField(rect, "Current Conditions:");
        //     }
        //
        //     private void DrawElementCallback(Rect rect, SerializedProperty property, int index, bool isactive,
        //         bool isfocused)
        //     {
        //         Rect r = rect;
        //         DrawPropertyField(ref r,
        //             _reorderableLists[property.propertyPath].serializedProperty.GetArrayElementAtIndex(index));
        //     }
        //
        //     private void DrawPropertyField(ref Rect r, SerializedProperty property, bool editRect = true)
        //     {
        //         r.height = EditorGUI.GetPropertyHeight(property, property.isExpanded);
        //         EditorGUI.PropertyField(r, property, new GUIContent(property.displayName));
        //         if (editRect)
        //
        //             r.y += r.height + _spacing;
        //     }
        //
        //
        //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //     {
        //         _totalHeight = 0;
        //         var it = GetFirstSerializedProperty(property);
        //         SerializedProperty arrayP = default;
        //         it.Next(true);
        //         while (it.NextVisible(false))
        //         {
        //             if (arrayP != null && it.propertyPath.Contains(arrayP.propertyPath))
        //             {
        //                 continue;
        //             }
        //
        //             if (it.propertyType == SerializedPropertyType.Generic)
        //             {
        //                 arrayP = it;
        //             }
        //
        //             _totalHeight += (SingleListHeight(property, it) + 25f);
        //         }
        //
        //         _totalHeight -= 120f;
        //
        //         return base.GetPropertyHeight(property, label) + _totalHeight + _extraHeight;
        //     }
        //
        //     private static SerializedProperty GetFirstSerializedProperty(SerializedProperty property)
        //     {
        //         SerializedProperty it = property.serializedObject.GetIterator();
        //         return it;
        //     }

        #endregion
    }
}