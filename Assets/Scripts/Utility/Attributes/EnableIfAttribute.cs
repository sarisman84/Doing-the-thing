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

    public class EnableIfAttribute : PropertyAttribute
    {
       
        public List<object> conditions = new List<object>();


        public EnableIfAttribute(params object[] args)
        {
            conditions.AddRange(args);
        }
    }
}