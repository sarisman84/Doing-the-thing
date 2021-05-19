using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public static class PickupManager
    {
        private static readonly List<Pickup> CurrentPickups = new();

        public static void RegisterPickup(Pickup pickup)
        {
            if (CurrentPickups.Contains(pickup)) return;
            CurrentPickups.Add(pickup);
        }

        public static Pickup FetchClosestPickup(Vector3 position, float minDistance)
        {
            Pickup result = CurrentPickups.Find(d =>
                Vector3.Distance(d.transform.position, position) <= minDistance && d.isActiveAndEnabled);
            return result;
        }

        public static void SetActive(this Pickup pickup, bool value)
        {
            if (CurrentPickups.Contains(pickup))
            {
                CurrentPickups.Remove(pickup);
            }

            pickup.gameObject.SetActive(value);
        }
    }
}