using System;
using Extensions;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.FireDefinitions
{
    [CreateAssetMenu(menuName = "Weapons/Fire Effects/HitScan", fileName = "New HitScan", order = 0)]
    public class HitScan : FireDefinition
    {
        public float hitScanDetectionRange;

        public override void Fire(Vector3 origin, Vector3 direction)
        {
            Color rayColor = Color.red;

            RaycastHit hitInfo;
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out hitInfo, hitScanDetectionRange))
            {
                targetSelectionType.TargetSelectionOnImpact(hitInfo.collider);
                CoroutineManager.Instance.StartCoroutine(targetSelectionType.impactEffect.PlayImpactEffect(hitInfo.point, hitInfo.normal));
                rayColor = Color.green;
            }

            Debug.DrawRay(ray.origin, ray.direction * hitScanDetectionRange, rayColor, 0.5f);
        }
    }
}