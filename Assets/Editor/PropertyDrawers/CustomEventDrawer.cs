// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Interactivity.Events;
// using Interactivity.Events.Listener;
// using TPUModelerEditor;
// using UnityEditor;
// using UnityEditorInternal;
// using UnityEngine;
// using Editor.Extension;
// using Interactivity;
//
// namespace Editor.PropertyDrawers
// {
//     [CustomEditor(typeof(EventListener))]
//     public class CustomEventDrawer : UnityEditor.Editor
//     {
//         private SerializedProperty _eventsProperty;
//         private ReorderableList _list;
//
//         private void OnEnable()
//         {
//             _eventsProperty = serializedObject.FindProperty("events");
//             _list = new ReorderableList(serializedObject, _eventsProperty, true, true, true, true);
//             _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, _eventsProperty.displayName);
//             _list.drawElementCallback = DrawElementCallback;
//             _list.elementHeightCallback = ElementHeightCallback;
//         }
//
//         private float ElementHeightCallback(int index)
//         {
//             float height = EditorUtilities.GetPropertyHeight();
//
//             SerializedProperty property = _list.serializedProperty.GetArrayElementAtIndex(index);
//             bool isNotNull = property.FindPropertyRelative("eventListener").objectReferenceValue != null;
//             bool isInstanceEvent = property.FindPropertyRelative("eventListener").objectReferenceValue is InstanceEvent;
//             bool useList = property.FindPropertyRelative("useCustomArg").boolValue;
//             bool useInteractor = property.FindPropertyRelative("useInteractor").boolValue;
//             if (!isNotNull) return height;
//             if (isInstanceEvent)
//             {
//                 height += EditorUtilities.GetPropertyHeight();
//                 if (useList && !useInteractor)
//                 {
//                     height += EditorUtilities.GetPropertyHeight(property.FindPropertyRelative("entityComparison"),
//                         EditorUtilities.ElementType.Array);
//                     height += EditorUtilities.GetPropertyHeight(property.FindPropertyRelative("instanceUnityEvent"),
//                         EditorUtilities.ElementType.UltEvent);
//                 }
//                 else
//                 {
//                     height += EditorUtilities.GetPropertyHeight(property.FindPropertyRelative("instanceUnityEvent"),
//                         EditorUtilities.ElementType.UltEvent);
//                 }
//             }
//             else
//             {
//                 height += EditorUtilities.GetPropertyHeight(property.FindPropertyRelative("defaultUnityEvent"),
//                     EditorUtilities.ElementType.UltEvent);
//             }
//
//
//             return height;
//         }
//
//
//         private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
//         {
//             SerializedProperty listener =
//                 _eventsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("eventListener");
//             Vector2 pos = rect.position;
//             Vector2 size = rect.size;
//             pos.x += 10f;
//             size.x -= 10f;
//             rect.position = pos;
//             rect.size = size;
//             EditorGUI.PropertyField(rect, _eventsProperty.GetArrayElementAtIndex(index),
//                 new GUIContent(listener.objectReferenceValue == null
//                     ? _eventsProperty.GetArrayElementAtIndex(index).displayName
//                     : listener.objectReferenceValue.name), true);
//         }
//
//         public override void OnInspectorGUI()
//         {
//             _list.DoLayoutList();
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
//
//
//     [CustomPropertyDrawer(typeof(EventDefinition))]
//     public class EventDefinitionAttribute : PropertyDrawer
//     {
//         private Dictionary<string, ReorderableList> _currentLists = new Dictionary<string, ReorderableList>();
//         private Dictionary<string, SerializedProperty> _foundProperties;
//
//         private SerializedObject _serializedObject;
//         private SerializedProperty _currentProperty;
//
//         private const string ComparisonList = "entityComparison";
//         private const string CompareEntities = "useCustomArg";
//         private const string UnityEvent = "defaultUnityEvent";
//         private const string InstanceEvent = "instanceUnityEvent";
//         private const string EventListener = "eventListener";
//         private const string CompareInteractor = "useInteractor";
//
//         private Rect _previousPosition;
//
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             _currentProperty = property;
//
//             _serializedObject = property.serializedObject;
//             _foundProperties = new Dictionary<string, SerializedProperty>();
//             _foundProperties = EditorUtilities.FindProperties(property, ComparisonList, CompareInteractor,
//                 CompareEntities, UnityEvent,
//                 InstanceEvent, EventListener);
//
//             if (!_currentLists.ContainsKey(property.displayName))
//             {
//                 ReorderableList list = new ReorderableList(_serializedObject, _foundProperties[ComparisonList], true,
//                     true, true,
//                     true);
//                 list.drawElementCallback = DrawElementCallback;
//                 list.drawHeaderCallback = DrawHeaderCallback;
//                 _currentLists.Add(property.displayName, list);
//             }
//
//
//             EditorGUI.BeginProperty(position, label, property);
//             OnInspectorGUI(_foundProperties, ref position, property);
//             EditorGUI.EndProperty();
//         }
//
//
//         private void OnInspectorGUI(Dictionary<string, SerializedProperty> foundProperties, ref Rect position,
//             SerializedProperty property)
//         {
//            
//             GUIContent eventLabel = new GUIContent(
//                 foundProperties[EventListener].displayName);
//             eventLabel.tooltip = EditorUtilities
//                 .TooltipBuilder(foundProperties[EventListener].objectReferenceValue == null, "Assign Event here", "The below methods get called when the event's InvokeMethod() gets called")
//                 .TooltipBuilder(foundProperties[EventListener].objectReferenceValue is InstanceEvent, "The below methods get called when the event's InvokeMethod() gets called as its assigned object is equal to one of the assigned objects.");
//
//             EditorUtilities.PropertyField(position, ref _previousPosition, foundProperties[EventListener],
//                 EditorUtilities.ElementType.Field, eventLabel, true);
//
//             if (foundProperties[EventListener].objectReferenceValue == null) return;
//             if (foundProperties[EventListener].objectReferenceValue is InstanceEvent)
//             {
//                 IInteractable interactable =
//                     (fieldInfo.GetValue(property.serializedObject.targetObject) as EventDefinition[] ??
//                      Array.Empty<EventDefinition>()).FirstOrDefault()?.Owner.GetComponent<IInteractable>();
//                 var interactorLabel = SetLabel(foundProperties, interactable);
//
//
//                 if (interactable == null)
//                 {
//                     DrawManualComparisonButton(foundProperties, ref position, property);
//                 }
//                 else if (!EditorUtilities.ToggleButton(position, ref _previousPosition, foundProperties[CompareInteractor],
//                     interactorLabel))
//                 {
//                     DrawManualComparisonButton(foundProperties, ref position, property);
//                 }
//
//
//                 EditorUtilities.PropertyField(position, ref _previousPosition,
//                     foundProperties[InstanceEvent],
//                     EditorUtilities.ElementType.UltEvent);
//                 return;
//             }
//
//             position = position.NewElementPosition(ref _previousPosition, EditorUtilities.ElementType.UltEvent,
//                 foundProperties[UnityEvent]);
//             EditorGUI.PropertyField(position, foundProperties[UnityEvent],
//                 new GUIContent(foundProperties[UnityEvent].displayName));
//             _serializedObject.ApplyModifiedProperties();
//         }
//
//         private void DrawManualComparisonButton(Dictionary<string, SerializedProperty> foundProperties, ref Rect position, SerializedProperty property)
//         {
//             GUIContent label = new GUIContent(foundProperties[CompareEntities].boolValue
//                 ? "Disable comparison"
//                 : "Enable Manual comparison");
//             if (EditorUtilities.ToggleButton(position, ref _previousPosition, foundProperties[CompareEntities],
//                 label))
//             {
//                 position = position.NewElementPosition(ref _previousPosition,
//                     EditorUtilities.ElementType.Array,
//                     foundProperties[ComparisonList]);
//                 _currentLists[property.displayName].DoList(position);
//             }
//         }
//
//
//         private static GUIContent SetLabel(Dictionary<string, SerializedProperty> foundProperties,
//             IInteractable interactable)
//         {
//             GUIContent interactorLabel = new GUIContent(foundProperties[CompareInteractor].boolValue
//                 ? "Disable Interactor comparison"
//                 : $"Compare Interactor");
//             interactorLabel.tooltip = foundProperties[CompareInteractor].boolValue
//                 ? "Disable Interactor comparison"
//                 : $"Use {interactable}'s latest interactor for comparison";
//             return interactorLabel;
//         }
//
//
//         private void DrawHeaderCallback(Rect rect)
//         {
//             EditorGUI.LabelField(rect, new GUIContent("Objects to Compare"));
//         }
//
//         private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
//         {
//             SerializedProperty element = _currentLists[_currentProperty.displayName].serializedProperty
//                 .GetArrayElementAtIndex(index);
//             EditorGUI.PropertyField(rect, element, new GUIContent(element.displayName));
//         }
//     }
// }