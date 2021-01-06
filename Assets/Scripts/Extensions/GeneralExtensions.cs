using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Player;
using Player.Weapons;
using UnityEditor;
using UnityEngine;
using static Extensions.GeneralExtensions.RadiusAxis;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class GeneralExtensions
    {
        public static T[] RemoveElements<T>(this T[] array, int amount, Func<bool> ifCondition = null)
        {
            T[] newArray = array;
            if (ifCondition != default)
                if (ifCondition.Invoke())
                {
                    newArray = new T[array.Length - amount];
                    Array.Copy(array, amount, newArray, 0, newArray.Length);
                }

            return newArray;
        }

        public static IEnumerable<int> GetIndexes<T>(this IEnumerable<T> list)
        {
            int[] result = new int[list.Count()];

            for (int index = 0; index < result.Length; index++)
            {
                result[index] = index;
            }

            return result;
        }

        public static Transform AddPosition(this Transform transform, Vector3 desiredValue)
        {
            Vector3 position = transform.position;
            position += desiredValue;
            transform.position = position;
            return transform;
        }


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

        public static IEnumerator GetDelayedRandomPositionInRange(this Vector3 vector3, float range, float delay,
            out Vector3 result)
        {
            result = GetRandomPositionInRange(vector3, range);
            return Delay(delay);
        }

        private static IEnumerator Delay(float delay)
        {
            yield return new WaitForSeconds(delay);
        }


        public static Vector3 GetRandomPositionInRange(this Vector3 vector3, float range, float delay,
            ref float counter, ref Vector3 lastResult, RadiusAxis axis = XZAxis)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0, delay);
            if (delay.Equals(counter))
            {
                lastResult = GetRandomPositionInRange(vector3, range, axis);
                counter = 0;
            }

            return lastResult;
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
                        if (hits[i].collider.GetComponent<PlayerController>())
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
            var applyAction = list.ToArray();
            foreach (var variable in applyAction)
            {
                method.Invoke(variable);
            }

            return applyAction;
        }

        public static SerializedProperty ApplyAction(this SerializedProperty value,
            Action<SerializedProperty> propertyMethod)
        {
            foreach (SerializedProperty p in value)
            {
                propertyMethod.Invoke(p.Copy());
            }


            return value;
        }

        public static List<bool> ApplyFunction<T>(this IEnumerable<T> list, Func<T, bool> method)
        {
            List<bool> result = new List<bool>();

            foreach (var obj in list)
            {
                result.Add(method?.Invoke(obj) ?? false);
            }

            return result;
        }

        public static IEnumerable<T> ApplyAction<T>(this IEnumerable<T> list, Action<T, int> method)
        {
            var applyAction = list.ToArray();
            for (var index = 0; index < applyAction.Length; index++)
            {
                var variable = applyAction[index];
                method.Invoke(variable, index);
            }

            return applyAction;
        }

        public static T Execute<T>(this T entity, Action<T> action)
        {
            if (entity != null)
            {
                action?.Invoke(entity);
            }

            return entity;
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

        public static bool NullcheckAndConfirm(this bool boolean, object value)
        {
            return boolean && !value.Equals(null);
        }


        public static List<Transform> GetChildrenWithTag<T>(this T value, string tag) where T : Component
        {
            List<Transform> list = new List<Transform>();
            for (int child = 0; child < value.transform.childCount; child++)
            {
                Transform trans = value.transform.GetChild(child);
                if (trans.Equals(null) && !trans.CompareTag(tag)) continue;
                list.Add(trans);
            }

            return list;
        }

        public static List<TComponent> GetComponent<TComponent>(this IEnumerable<Component> value)
            where TComponent : Component
        {
            List<TComponent> list = new List<TComponent>();

            foreach (var var in value)
            {
                list.Add(var.GetComponent<TComponent>());
            }

            return list;
        }

        public static Transform GetChildWithTag<T>(this T value, string tag) where T : Component
        {
            Transform result = null;

            for (int child = 0; child < value.transform.childCount; child++)
            {
                if (value.transform.GetChild(child).CompareTag(tag))
                {
                    result = value.transform.GetChild(child);

                    break;
                }

                result = GetChildWithTag(value.transform.GetChild(child), tag);
                if (result && result.CompareTag(tag)) break;
            }

            return result;
        }


        public static bool IsInTheVicinityOf(this Vector3 ogPosition, Vector3 targetPos, float range = 1f)
        {
            return ogPosition.x >= targetPos.x - range
                   && ogPosition.x <= targetPos.x + range
                   && ogPosition.y >= targetPos.y - range &&
                   ogPosition.y <= targetPos.y + range
                   && ogPosition.z >= targetPos.z - range &&
                   ogPosition.z <= targetPos.z + range;
        }
    }
}