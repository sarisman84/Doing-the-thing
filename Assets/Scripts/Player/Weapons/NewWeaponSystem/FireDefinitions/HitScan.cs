using System;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.FireDefinitions
{
    [CreateAssetMenu(menuName = "Weapons/Fire Effects/HitScan", fileName = "New HitScan", order = 0)]
    public class HitScan : FireDefinition
    {
        public float hitScanDetectionRange;

        public override void FireCustomEffect(Vector3 origin, Vector3 direction,
            Func<Collider, string> onImpactCallback)
        {
            Color rayColor = Color.red;

            RaycastHit hitInfo;
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out hitInfo, hitScanDetectionRange))
            {
                onImpactCallback?.Invoke(hitInfo.collider);
                rayColor = Color.green;
            }

            Debug.DrawRay(ray.origin, ray.direction * hitScanDetectionRange, rayColor, 0.5f);
        }
    }
}