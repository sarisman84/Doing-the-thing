using System;
using System.Collections.Generic;
using UnityEngine;

namespace General_Scripts.Utility.Extensions
{
    public static class TransformExtensions
    {
        public static Transform GetChildWithTag(this Transform transform, string tag, bool debug = false)
        {
            Transform result = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.childCount > 0)
                {
                    result = GetChildWithTag(child, tag);
                    if (result)
                        return result;
                }

                if (child.CompareTag(tag))
                {
                    result = child;
                    if (result)
                        return result;
                }
            }

            if (debug)
                Debug.Log(
                    $"Searched {transform.name}'s children. Found {transform.childCount} children. Result is {result}");

            return result;
        }

        public static Transform GetFirstActiveChild(this Transform transform, bool searchChildrenToo = false)
        {
            Transform result = null;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.childCount > 0 && searchChildrenToo)
                {
                    result = GetFirstActiveChild(child, searchChildrenToo);
                    if (result) return result;
                }

                if (child.gameObject.activeSelf)
                {
                    result = child;
                    break;
                }
            }


            return result;
        }

        public static IEnumerable<Transform> GetChildren(this Transform transform, bool getAllChildren = false)
        {
            List<Transform> result = new List<Transform>();


            for (int i = 0; i < transform.childCount; i++)
            {
                result.Add(transform.GetChild(i));
                if (getAllChildren)
                    result.AddRange(GetChildren(result[i], getAllChildren));
            }

            return result;
        }

        public static Transform GetTheClosestEntityOfType<T>(this Vector3 centre, float radius) where T: Component
        {
            Collider[] foundTargets = Physics.OverlapSphere(centre, radius);

            float minDist = float.MaxValue;
            Transform result = null;
            foreach (var target in foundTargets)
            {
                float dist = Vector3.Distance(centre, target.transform.position);
                if (minDist < dist)
                {
                    result = target.transform;
                    minDist = dist;
                }
                
            }
            return result;
        }
    }
}