using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ExposeAttribute))]
    public class ScriptableExposerDrawer : PropertyDrawer
    {
        bool displaySubEditor = false;
        private SerializedObject _so;
        float _totalHeight;

        private Rect _areaRect;
        private float _spacing = 2.5f;

        private float _boxSizeOffset = 15f;

        private readonly Dictionary<string, ReorderableList> _reorderableListDictionary =
            new Dictionary<string, ReorderableList>();


        //0.95f,0.62f
        //Color.HSVToRGB(0, 0.95f, 0.42f);
        // public Color BackgroundColor =>
        //     EditorGUIUtility.isProSkin
        //         ? GUI.backgroundColor - new Color(0.8f, 0.8f, 0.8f, 0f)
        //         : Color.HSVToRGB(0, 0.95f, 0.82f);

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            //EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            ExposeScriptableObject(property, position, ref _so);
        }


        private bool IsCorrectType(Type firstValue, Type secondValue)
        {
            return firstValue.IsSubclassOf(secondValue) || firstValue == secondValue;
        }

        private bool IsCorrectType(SerializedProperty element, Type secondValue)
        {
            if (element.objectReferenceValue == default ||
                element.propertyType != SerializedPropertyType.ObjectReference)
                return false;

            return element.objectReferenceValue.GetType().IsSubclassOf(secondValue) ||
                   element.objectReferenceValue.GetType() == secondValue;
        }

        private void ExposeScriptableObject(SerializedProperty property, Rect position, ref SerializedObject so)
        {
            Rect r = position;
            r.y += 5f;
            int ogIndentLevel = EditorGUI.indentLevel;
            r.height = EditorGUI.GetPropertyHeight(property, property.isExpanded);
            EditorGUI.PropertyField(r, property, property.isExpanded);

            if (property.objectReferenceValue != null)
            {
                Rect offsetRect = r;
                offsetRect.x = EditorGUIUtility.labelWidth - (15f * EditorGUI.indentLevel);
                // EditorGUI.indentLevel -= EditorGUI.indentLevel * 15;
                displaySubEditor = EditorGUI.Toggle(offsetRect, displaySubEditor);

                //EditorGUI.indentLevel = ogIndentLevel;
            }


            if (!IsCorrectType(property, typeof(ScriptableObject)))
            {
                return;
            }

            if (so == null)
            {
                so = new SerializedObject(property.objectReferenceValue);
            }

            if (!displaySubEditor)
            {
                return;
            }

            r.y += r.height;
            GUI.Box(
                new Rect(r.x + OffsetByIndentLevel(_boxSizeOffset), r.y, r.width - OffsetByIndentLevel(_boxSizeOffset),
                    _totalHeight - 10f), GUIContent.none, "box");
            SerializedProperty arrayP = default;
            SerializedProperty it = so.GetIterator();
            it.Next(true);


            EditorGUI.indentLevel++;

            bool disableFirstElement = true;
            bool indentOnSecondElement = false;


            while (it.NextVisible(false))
            {
                if (indentOnSecondElement)
                {
                    EditorGUI.indentLevel++;
                    indentOnSecondElement = false;
                }

                if (disableFirstElement)
                {
                    r.y += _spacing;
                    indentOnSecondElement = true;
                }

                if (arrayP != null && it.propertyPath.Contains(arrayP.propertyPath))
                {
                    continue;
                }

                if (it.propertyType == SerializedPropertyType.Generic && it.isArray)
                {
                    //DrawReorderableList(disableFirstElement, ref r, it);
                    arrayP = it;
                }


                DrawSingleElement(disableFirstElement, ref r, it);
                disableFirstElement = false;
                it.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = ogIndentLevel;
            property.serializedObject.ApplyModifiedProperties();
        }

        private static float OffsetByIndentLevel(float offset)
        {
            return offset * EditorGUI.indentLevel;
        }

        private void DrawReorderableList(bool disableFirstElement, ref Rect rect,
            SerializedProperty it)
        {
            string key = it.Copy().propertyPath;
            if (!_reorderableListDictionary.ContainsKey(key))
            {
                ReorderableList list = new ReorderableList(_so, it, true, true, true, true);
                list.elementHeightCallback += index => ElementHeightCallback(list, index);
                list.drawElementCallback += (elementRect, index, isActive, isFocused) =>
                    DrawElementCallback(elementRect, index, isActive, isFocused, list);
                list.drawHeaderCallback += (headerRect) => DrawHeaderCallback(headerRect, it);
                _reorderableListDictionary.Add(key, list);
            }

            //This DoList call back also throws the type error as well as the operation thingy.
            _reorderableListDictionary[key].DoList(rect);
        }

        private void DrawHeaderCallback(Rect rect, SerializedProperty property)
        {
            EditorGUI.LabelField(rect, new GUIContent(property.displayName));
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused,
            ReorderableList reorderableList)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

            if (element.CountInProperty() == 1 &&
                IsCorrectType(element, typeof(ScriptableObject)))
            {
                SerializedObject localSo = new SerializedObject(element.objectReferenceValue);
                ExposeScriptableObject(element, rect, ref localSo);
            }
            else if (element.CountInProperty() > 1)
            {
                foreach (SerializedProperty p in element)
                {
                    DrawSingleElement(false, ref rect, p);
                }
            }
        }


        private float ElementHeightCallback(ReorderableList list, int index)
        {
            SerializedObject localSo = default;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            if (element.CountInProperty() == 1 &&
                IsCorrectType(element, typeof(ScriptableObject)))
            {
                localSo = new SerializedObject(element.objectReferenceValue);
            }

            if (CalculatePropertyHeight(element, localSo, out var propertyHeight, false)) return propertyHeight;

            propertyHeight += EditorGUI.GetPropertyHeight(element, element.isExpanded);
            foreach (SerializedProperty p in element)
            {
                propertyHeight += EditorGUI.GetPropertyHeight(p, p.isExpanded) + _spacing;
            }

            return EditorGUI.GetPropertyHeight(element, element.isExpanded) + propertyHeight;
        }

        private void DrawSingleElement(bool disableFirstElement, ref Rect r, SerializedProperty it)
        {
            EditorGUI.BeginDisabledGroup(disableFirstElement);
            r.height = EditorGUI.GetPropertyHeight(it, it.isExpanded) +
                       _spacing;
            EditorGUI.PropertyField(r, it, it.isExpanded);
            EditorGUI.EndDisabledGroup();
            r.y += r.height + _spacing;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null && IsCorrectType(fieldInfo.FieldType, typeof(ScriptableObject)))
            {
                _so = new SerializedObject(property.objectReferenceValue);
            }

            if (CalculatePropertyHeight(property, _so, out var propertyHeight)) return propertyHeight;

            return base.GetPropertyHeight(property, label) + 5f;
        }

        private bool CalculatePropertyHeight(SerializedProperty property, SerializedObject so, out float propertyHeight,
            bool isLocal = true)
        {
            GUIContent label = default;
            if (so != null && displaySubEditor)
            {
                _totalHeight = 0;
                _totalHeight += 15f;
                SerializedProperty arrayP = default;
                SerializedProperty it = so.GetIterator();

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

                    _totalHeight += EditorGUI.GetPropertyHeight(it, it.isExpanded) +
                                    _spacing * (it.isExpanded ? it.Copy().CountRemaining() + 1f : 1 + 2);
                }

                propertyHeight = isLocal
                    ? base.GetPropertyHeight(property, label) + _totalHeight
                    : EditorGUI.GetPropertyHeight(property, property.isExpanded) + _totalHeight;
                return true;
            }

            propertyHeight = 0;
            return false;
        }
    }
}