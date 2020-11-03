using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Player;
using Player.Weapons;
using UnityEngine;
using static Extensions.GeneralExtensions.RadiusAxis;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class GeneralExtensions
    {
        public static void ChangeSize<T>(this Collider collider, T value)
        {
            BoxCollider boxCol = collider.GetComponent<BoxCollider>();
            SphereCollider sphereCol = collider.GetComponent<SphereCollider>();
            CapsuleCollider capsuleCol = collider.GetComponent<CapsuleCollider>();

            if (boxCol != null)
            {
                if (value is Vector3)
                    boxCol.size = (Vector3) Convert.ChangeType(value, typeof(Vector3));
                if (value is float || value is int)
                {
                    float size = (float) Convert.ChangeType(value, typeof(float));
                    boxCol.size = new Vector3(size, size, size);
                }
            }

            if (sphereCol != null)
            {
                if (value is Vector3)
                    sphereCol.radius = ((Vector3) Convert.ChangeType(value, typeof(Vector3))).y;
                if (value is float || value is int)
                {
                    float size = (float) Convert.ChangeType(value, typeof(float));
                    sphereCol.radius = size;
                }
            }

            if (capsuleCol != null)
            {
                if (value is Vector3)
                    capsuleCol.radius = ((Vector3) Convert.ChangeType(value, typeof(Vector3))).y;
                if (value is float || value is int)
                {
                    float size = (float) Convert.ChangeType(value, typeof(float));
                    capsuleCol.radius = size;
                }
            }
        }

        public enum RadiusAxis
        {
            XAxis,
            YAxis,
            ZAxis,
            XYAxis,
            XZAxis,
            YZAxis,
            XYZAxis
        }

        public static Vector3 GetRandomPositionInRange(this Vector3 vector3, float d,
            RadiusAxis axis = XZAxis)
        {
            switch (axis)
            {
                case XAxis:
                    return new Vector3(vector3.x + Random.Range(-d, d), vector3.y, vector3.z);

                case YAxis:
                    return new Vector3(vector3.x, vector3.y + Random.Range(-d, d), vector3.z);
                case ZAxis:
                    return new Vector3(vector3.x, vector3.y, vector3.z + Random.Range(-d, d));

                case XYAxis:

                    return new Vector3(vector3.x + Random.Range(-d, d), vector3.y + Random.Range(-d, d), vector3.z);

                case XZAxis:
                    return new Vector3(vector3.x + Random.Range(-d, d), vector3.y, vector3.z + Random.Range(-d, d));

                case YZAxis:
                    return new Vector3(vector3.x, vector3.y + Random.Range(-d, d), vector3.z + Random.Range(-d, d));

                case XYZAxis:
                    return new Vector3(vector3.x + Random.Range(-d, d), vector3.y + Random.Range(-d, d),
                        vector3.z + Random.Range(-d, d));
            }

            return vector3;
        }


        public static RaycastHit GetClosestHit(this RaycastHit[] hits)
        {
            RaycastHit closestHit = new RaycastHit();
            float minDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (minDistance > hits[i].distance)
                {
                    if (hits[i].collider)
                        if (hits[i].collider.GetComponent<FirstPersonController>())
                            continue;
                    if (hits[i].distance == 0) continue;
                    minDistance = hits[i].distance;
                    closestHit = hits[i];
                }
            }

            return closestHit;
        }


        public static IEnumerable<T> ApplyAction<T>(this IEnumerable<T> list, Action<T> method)
        {
            var applyAction = list as T[] ?? list.ToArray();
            foreach (var variable in applyAction)
            {
                method.Invoke(variable);
            }

            return applyAction;
        }


        public static T ApplyAction<T>(this T value, Action<T> method)
        {
            if (!value.Equals(null))
            {
                method.Invoke(value);
            }

            return value;
        }


        public static void SetGameObjectAsChild(this Transform transform, GameObject newModel)
        {
            newModel.transform.SetParent(transform);
            newModel.SetActive(true);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;
        }


        public static List<int> ToInteger(this LayerMask layerMask)
        {
            List<int> layers = new List<int>();
            var bitmask = layerMask.value;
            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & bitmask) != 0)
                {
                    layers.Add(i);
                }
            }

            return layers;
        }

        public static void RemoveListener(this Func<object, object> thisAction, Func<object, object> listener)
        {
            thisAction -= listener;
        }


        public static Vector3 TryGetValue(this Vector3 value, params object[] args)
        {
            if (!value.Equals(Vector3.zero))
            {
                return value;
            }
            var transform = ((Transform) Array.Find(args, o => o is Transform));
            var forward = transform.forward;
            Vector3 originFacingDirection = forward * 100f;
            Ray ray = new Ray(transform.position, forward.normalized);
            float info = 0f;
            new Plane(Vector3.forward, originFacingDirection).Raycast(ray, out info);

            Vector3 point = ray.GetPoint(info);
            return point;
        }
    }
}