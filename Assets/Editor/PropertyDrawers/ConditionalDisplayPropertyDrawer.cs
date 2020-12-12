using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Utility.Attributes;

namespace Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class ConditionalDisplayPropertyDrawer : PropertyDrawer
    {
        private EnableIfAttribute _enableIfAttribute;
        private Dictionary<SerializedPropertyType, List<SerializedProperty>> _foundSerializedProperties;

        private List<object> _variables;

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            _enableIfAttribute = attribute as EnableIfAttribute;

            //Create dictionary to store the found property types in the argument.
            _foundSerializedProperties = new Dictionary<SerializedPropertyType, List<SerializedProperty>>();

            //Create object list that stores all the found values from both the properties and the static arguments present in the attribute.
            _variables = new List<object>();


            _enableIfAttribute?.conditions.ApplyAction(o =>
            {
                //Get the property and and its value.
                switch (o)
                {
                    case string p:
                        SerializedProperty foundProperty = property.serializedObject.FindProperty(p);
                        if (foundProperty != null)
                            switch (foundProperty.propertyType)
                            {
                                case SerializedPropertyType.Integer:
                                    _variables.Add(foundProperty.intValue);

                                    break;
                                case SerializedPropertyType.Boolean:
                                    _variables.Add(foundProperty.boolValue);

                                    break;
                                case SerializedPropertyType.Float:
                                    _variables.Add(foundProperty.floatValue);

                                    break;
                                case SerializedPropertyType.String:
                                    _variables.Add(foundProperty.stringValue);

                                    break;
                                case SerializedPropertyType.Color:
                                    _variables.Add(foundProperty.colorValue);

                                    break;
                                case SerializedPropertyType.ObjectReference:
                                    _variables.Add(foundProperty.objectReferenceValue.name);

                                    break;
                                case SerializedPropertyType.Enum:
                                    _variables.Add(foundProperty.enumValueIndex);

                                    break;
                            }

                        else
                            _variables.Add(p);

                        break;
                    //Check if the enum is an Operator and store said enum as an Operator. Otherwise, store its index
                    case Enum enumVal:
                        if (enumVal is Operator op)
                        {
                            _variables.Add(op);
                            break;
                        }

                        _variables.Add(Convert.ToInt32(enumVal));
                        break;

                    //Store the color if was given as an argument
                    case Color color:
                        _variables.Add(color);
                        break;
                }
            });

            bool canDisplay = false;

            //Create a list of lists that contain the results.
            List<List<bool>> result = new List<List<bool>>();

            //Create a list of operators.
            List<Operator> operators = new List<Operator>();
            int currentList = 0;


            //This for-loop increments by 3 as it will be looking at the next 3 elements, starting from i.
            //Look order: i > i+1 > i+2 (example 0, 1 ,2)
            //It also contains a check to see if the list is less or equal to 3, in which the loop will do a normal check on the element list size. Otherwise, it will check if the list count - 3 element offset is less or equal to the current index.
            for (int i = 0; _variables.Count >= 3 ? i <= _variables.Count - 3 : i < _variables.Count; i = i + 3)
            {
                result.Add(new List<bool>());
                object a;
                object b = null;
                object c = null;
                a = _variables[i];


                //The below if statements check if the next index is not outside of range.

                if (i + 1 <= _variables.Count - 1)
                    b = _variables[i + 1]; // 1

                if (i + 2 <= _variables.Count - 1)
                    c = _variables[i + 2]; // 2

                //This one checks whenever or not the 4th element is an operator, in which we store said operator when found.
                if (i + 4 <= _variables.Count - 1)
                {
                    object d = _variables[i + 4];
                    if (d is Operator newOp)
                    {
                        operators.Add(newOp);
                    }
                }


                //If only one argument exists, try to find a bool and get its value (since only bool variables can exist in just one argument)
                if (b == null || c == null)
                {
                    switch (a)
                    {
                        case bool value:
                            result[currentList].Add(value);
                            break;
                    }

                    //If the second element is not null, assume that the first element is an operator and try to find the Not Operator. Then, try to find if b is bool and store an inverted version of its value.
                    if (b != null)
                    {
                        if (a is Operator o)
                            switch (o)
                            {
                                case Operator.Not:
                                    if (b is bool moreValue)
                                    {
                                        result[currentList].Add(!moreValue);
                                    }


                                    break;
                            }

                        break;
                    }
                }

                //If the second value is an operator and all 3 values are not null, compare the first value with the third value and evaluate a true/false result based on the operator.
                if (b != null && b is Operator op && a != null && c != null)
                {
                    switch (op)
                    {
                        case Operator.Equals:


                            if (a is int val1 && c is int val2)
                            {
                                result[currentList].Add(val1 == val2);
                            }
                            else if (a is string text1 && c is string text2)
                            {
                                result[currentList].Add(text1.Equals(text2));
                            }
                            else if (a is Color color1 && c is Color color2)
                            {
                                result[currentList].Add(color1.Equals(color2));
                            }
                            else if (a is float float1 && c is float float2)
                            {
                                result[currentList].Add(float1.Equals(float2));
                            }

                            break;
                        case Operator.And:

                            if (a is bool bool1 && c is bool bool2)
                            {
                                result[currentList].Add(bool1 && bool2);
                            }

                            break;
                        case Operator.Or:
                            if (a is bool b1 && c is bool b2)
                            {
                                result[currentList].Add(b1 || b2);
                            }

                            break;
                        default:
                            break;
                    }
                }


                currentList++;
            }


            //If there is only one list of results, do a single check.
            if (result.Count == 1)
            {
                canDisplay = result[0].All(r => r);
            }
            else
            {
                int nextOperator = 0;

                //Similar to the previous for-loop, this loop checks itself and the next element per iteration.
                for (int j = 0; j <= result.Count - 2; j += 2)
                {
                    bool r1 = result[j].All(r => r);
                    bool r2 = result[j + 1].All(r => r);
                    Debug.Log($"If {r1} {operators[nextOperator].ToString()} {r2} are true");

                    switch (operators[nextOperator])
                    {
                        case Operator.And:
                            canDisplay = r1 && r2;
                            break;
                        case Operator.Or:
                            canDisplay = r1 || r2;
                            break;
                    }

                    //Increment the next operator and make sure that it does not get out of range from its list.
                    nextOperator++;
                    nextOperator = Mathf.Clamp(nextOperator, 0, operators.Count - 1);
                }
            }

            //Disable/Enable the field depending on the result of the above evaluations


            EditorGUI.BeginDisabledGroup(!canDisplay);


            EditorGUI.PropertyField(position, property, label, true);
            property.serializedObject.ApplyModifiedProperties();


            EditorGUI.EndDisabledGroup();
        }

        //Scale the field height depending whenever the property itself is expanded.
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            if (property.isExpanded)
                height = SetHeight(property, height);


            height += 10f;
            return height;
        }

        private float SetHeight(SerializedProperty property, float height)
        {
            foreach (SerializedProperty p in property)
            {
                if (p.isExpanded && p.hasVisibleChildren)
                {
                    SerializedProperty foundProp = p.Copy();
                    height = SetHeight(foundProp, height);
                }


                height += base.GetPropertyHeight(p, new GUIContent(p.displayName));
            }


            return height;
        }
    }
}