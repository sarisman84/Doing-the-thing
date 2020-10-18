using System;
using Player;
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
    }
}