using System;
using System.Globalization;
using Player;
using UnityEngine;
using static Extensions.GeneralExtensions.RadiusAxis;
using Random = System.Random;

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

        public static Vector3 GetRandomPositionInRange(this Vector3 vector3, int d,
            RadiusAxis axis = XZAxis)
        {
            
            Random rnd = new Random();
            
          

            int newPos = rnd.Next(-d, d);
            switch (axis)
            {
                case XAxis:
                    return new Vector3(vector3.x + newPos, vector3.y, vector3.z);
                
                case YAxis:
                    return new Vector3(vector3.x, vector3.y +  newPos, vector3.z);
                case ZAxis:
                    return new Vector3(vector3.x, vector3.y, vector3.z + newPos);
                
                case XYAxis:
                    
                   return new Vector3(vector3.x +  newPos, vector3.y +  newPos, vector3.z);
                
                case XZAxis:
                    return new Vector3(vector3.x +  newPos, vector3.y, vector3.z +  newPos);
                
                case YZAxis:
                    return new Vector3(vector3.x, vector3.y +  newPos, vector3.z +  newPos);
                
                case XYZAxis:
                    return new Vector3(vector3.x +  newPos, vector3.y +  newPos, vector3.z +  newPos);
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
    }
}