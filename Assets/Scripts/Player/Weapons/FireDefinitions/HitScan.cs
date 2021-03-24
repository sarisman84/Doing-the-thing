using System;
using System.Collections;
using Extensions;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player.Weapons.NewWeaponSystem.FireDefinitions
{
    [CreateAssetMenu(menuName = "Weapons/Fire Type/HitScan", fileName = "New HitScan", order = 0)]
    public class HitScan : FireType
    {
        public float hitScanDetectionRange;
        public LayerMask hitScanLayerMask;
        public TrailRenderer fireParticle;


        private void OnEnable()
        {
        }

        public override void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            Color rayColor = Color.red;

            RaycastHit hitInfo;
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out hitInfo, hitScanDetectionRange, hitScanLayerMask))
            {
                TrailRenderer fx = ObjectManager.DynamicComponentInstantiate(fireParticle, true);

                fx.transform.position = origin;
                fx.AddPosition(origin);
                targetSelectionType.TargetSelectionOnImpact(hitInfo.collider, owner);
                if (targetSelectionType.impactEffect)
                    CoroutineManager.Instance.DynamicStartCoroutine(
                        targetSelectionType.impactEffect.PlayImpactEffect(hitInfo.point, hitInfo.normal));
                fx.transform.position = hitInfo.point;
                CoroutineManager.Instance.DynamicStartCoroutine(ResetTrailFX(fx));

                //fireParticle.DynamicPlay();
                rayColor = Color.green;
            }

            Debug.DrawRay(ray.origin, ray.direction * hitScanDetectionRange, rayColor, 0.5f);
        }

        private IEnumerator ResetTrailFX(TrailRenderer fx)
        {
            yield return new WaitForSeconds(1);
            fx.Clear();
            fx.gameObject.SetActive(false);
        }
    }
}