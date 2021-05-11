using UnityEngine;

namespace General_Scripts.Utility.Extensions
{
    public static class Vector3Extensions
    {
        public static bool IsInTheVicinityOf(this Vector3 position, Vector3 targetPosition, float minDistance)
        {
            return (targetPosition - position).magnitude <= minDistance;
        }
    }
}