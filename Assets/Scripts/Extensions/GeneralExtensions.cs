using System;
using UnityEngine;

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
                boxCol.size = (Vector3) Convert.ChangeType(value, typeof(Vector3));
            }

            if (sphereCol != null)
            {
                sphereCol.radius = ((Vector3) Convert.ChangeType(value, typeof(Vector3))).y;
            }

            if (capsuleCol != null)
            {
                capsuleCol.radius = ((Vector3) Convert.ChangeType(value, typeof(Vector3))).y;
            }
        }
    }
}