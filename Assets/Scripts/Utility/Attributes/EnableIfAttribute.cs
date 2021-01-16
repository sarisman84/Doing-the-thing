using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Attributes
{
    public enum Operator
    {
        And,
        Or,
        Equals,
        Not
    }

    public enum Class
    {
        Base,
        All
    }

    /// <summary>
    /// Allows you to enable (or disable) a field in the inspector by using a boolean for its condition.
    /// Values are assigned using a string (and its case-sensitive).
    /// (Use the Operator enum for more expressive conditions)
    /// </summary>
    public class EnableIfAttribute : PropertyAttribute
    {
       
        public List<object> conditions = new List<object>();


        public EnableIfAttribute(params object[] args)
        {
            conditions.AddRange(args);
        }
    }


    public class TypeCondition
    {
        public Type registeredType;
        public TypeCondition(Type type)
        {
            registeredType = type;
        }
    }
}