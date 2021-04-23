using System;
using UnityEngine;

namespace General_Scripts.Utility.Extensions
{
    public static class TransformExtensions
    {
        public static Transform GetChildWithTag(this Transform transform, string tag)
        {
            Transform result = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.childCount > 0)
                    result = GetChildWithTag(child, tag);
                if (child.CompareTag(tag))
                    result = child;
               
            }

            return result;
        }
    }
}