using System;
using System.Linq;
using General_Scripts.Utility.Extensions;
using Scripts;
using UnityEngine;

namespace Player.Scripts
{
    public class PickupInteractor : MonoBehaviour
    {
        public float pickupRange = 3f;
        public float pickupRate = 0.5f;
        public bool showDebug;

        private float currentRate;

        private void Update()
        {
            CheckForSurroundingPickups(pickupRange, pickupRate);
        }

        private void CheckForSurroundingPickups(float radius, float rate)
        {
            currentRate += Time.deltaTime;
            currentRate = Mathf.Clamp(currentRate, 0, rate);

            if (currentRate == rate)
            {
                PickupManager.FetchClosestPickup(transform.position, radius)?.OnPickup(this);
                currentRate = 0;
            }
        }


        private void OnDrawGizmos()
        {
            if (showDebug)
            {
                Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position, pickupRange);
            }
        }
    }
}