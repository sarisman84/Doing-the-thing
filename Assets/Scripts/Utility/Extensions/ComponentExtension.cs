using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General_Scripts.Utility.Extensions
{
    public static class ComponentExtension
    {
        public static IEnumerable<T> GetComponent<T>(this IEnumerable<Collider> colliders)
        {
            List<T> result = new List<T>();

            foreach (var collider in colliders)
            {
                T var = collider.GetComponent<T>();
                if (var != null)
                    result.Add(var);
            }

            return result;
        }
    }
}