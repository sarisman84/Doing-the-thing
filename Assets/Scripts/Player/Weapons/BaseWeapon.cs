using Interactivity;
using Managers;
using UnityEngine;

namespace Player.Weapons
{
    [System.Serializable]
    public abstract class BaseWeapon
    {
        public abstract void PrimaryFire();
        public abstract void SecondaryFire();

        public BaseWeapon(WeaponVisualiser visualiser)
        {
            this.visualiser = visualiser;
        }

        public WeaponVisualiser visualiser;

        public static RaycastHit[] Hitscan(Vector3 firePosition, Vector3 fireDirection, int penetration = 1)
        {
            RaycastHit[] hits = new RaycastHit[penetration];
            Physics.RaycastNonAlloc(firePosition, fireDirection, hits);

            return hits;
        }
    }

    public class TestingWeapon : BaseWeapon
    {
        private float _localFireRate;
        private float fireRate = 0.15f;

        public override void PrimaryFire()
        {
            _localFireRate += Time.deltaTime;
            _localFireRate = Mathf.Clamp(_localFireRate, 0, fireRate);

            //ParticleEffectsManager.PublicInstance.FindEffect("muzzleFlash") ?.Play(visualiser.WeaponBarrel.position, visualiser.WeaponBarrel.forward);

            if (_localFireRate.Equals(fireRate))
            {
                var position = visualiser.WeaponBarrel.position;
                var direction = visualiser.WeaponBarrel.forward;
                RaycastHit[] result = Hitscan(position, direction);
                float distance = result[0].distance <= 0 ? 1000f : result[0].distance;
                Debug.DrawRay(position, direction * distance, Color.red, 0.5f);
                if (!result.Equals(null))
                {
                    foreach (var t in result)
                    {
                        if (t.collider)
                            t.collider.GetComponent<IDamageable>()?.TakeDamage(1);
                    }
                }


                _localFireRate = 0;
            }
        }

        public override void SecondaryFire()
        {
            throw new System.NotImplementedException();
        }

        public TestingWeapon(WeaponVisualiser visualiser) : base(visualiser)
        {
            visualiser.SetWeaponModel("testWeaponModel",
                Object.Instantiate(Resources.Load<GameObject>("WeaponModels/Test_Pistol")));
        }
    }
}