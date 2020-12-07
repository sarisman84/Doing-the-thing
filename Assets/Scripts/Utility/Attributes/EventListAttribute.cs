using System;
using UnityEngine;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EventListAttribute : PropertyAttribute
    {
    }
}