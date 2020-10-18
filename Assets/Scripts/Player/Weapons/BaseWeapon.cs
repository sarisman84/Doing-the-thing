using Cinemachine;
using Extensions;
using Interactivity;
using Managers;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons
{
    [System.Serializable]
    public abstract class BaseWeapon
    {
        public abstract void PrimaryFire();
        public abstract void SecondaryFire();

        public BaseWeapon(WeaponVisualiser visualiser, CinemachineFreeLook playerCamera)
        {
            this.visualiser = visualiser;
            this.playerCamera = playerCamera;
        }

        public WeaponVisualiser visualiser;
        protected CinemachineFreeLook playerCamera;

        public static RaycastHit[] Hitscan(Vector3 firePosition, Vector3 fireDirection)
        {
            RaycastHit[] hits = new RaycastHit[10];
            Physics.RaycastNonAlloc(firePosition, fireDirection, hits);

            return hits;
        }
    }

    public class TestingWeapon : BaseWeapon
    {
        private float _localFireRate;
        private float fireRate = 0.1f;
        private TrailRenderer bulletTrail;

        public override void PrimaryFire()
        {
            _localFireRate += Time.deltaTime;
            _localFireRate = Mathf.Clamp(_localFireRate, 0, fireRate);


            if (_localFireRate.Equals(fireRate))
            {
                var transform = playerCamera.transform;
                var position = transform.position;
                var forward = transform.forward;
                if (bulletTrail)
                {
                    bulletTrail.Clear();
                    bulletTrail.transform.position = Vector3.zero;
                    
                }

                bulletTrail = bulletTrail
                    ? bulletTrail
                    : Object.Instantiate(Resources.Load<TrailRenderer>("WeaponEffects/Bullet Trail"));
                bulletTrail.transform.position = visualiser.WeaponBarrel.position;

                RaycastHit hit = Hitscan(position, forward).GetClosestHit();
                if (hit.collider)
                {
                    hit.collider.GetComponent<IDamageable>()?.TakeDamage(1);
                    bulletTrail.AddPosition(hit.point);
                }
                else
                {
                    bulletTrail.AddPosition(forward * 100f);
                }
           

                
            

                Debug.DrawRay(position, forward * hit.distance, Color.red);
                _localFireRate = 0;
            }
        }

        public override void SecondaryFire()
        {
            throw new System.NotImplementedException();
        }

        public TestingWeapon(WeaponVisualiser visualiser, CinemachineFreeLook camera) : base(visualiser, camera)
        {
            visualiser.SetWeaponModel("testWeaponModel",
                Object.Instantiate(Resources.Load<GameObject>("WeaponModels/Test_Pistol")));
        }
    }
}