using System;
using Cinemachine;
using Extensions;
using Interactivity;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player.Weapons
{
    [System.Serializable]
    public abstract class BaseWeapon
    {
        public abstract void PrimaryFire();
        public abstract void SecondaryFire();


        public BaseWeapon(WeaponVisualiser visualiser, CinemachineFreeLook playerCamera, HudManager uiVisualer)
        {
            this.visualiser = visualiser;
            this.playerCamera = playerCamera;
            this.uiVisualer = uiVisualer;
        }

        public WeaponVisualiser visualiser;
        protected CinemachineFreeLook playerCamera;
        public HudManager uiVisualer;

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
        private const float FireRate = 0.1f;
        private const int MagazineClip = 10000;
        private int _currentBulletCount = 0;

        public int AddCurrentMagazineClip
        {
            set => _currentBulletCount = _currentBulletCount >= MagazineClip ? _currentBulletCount : _currentBulletCount + value;
        }

        public bool AddAmmo(int amount)
        {
            if (_currentBulletCount < MagazineClip)
            {
                _currentBulletCount += amount;
                _currentBulletCount = Mathf.Clamp(_currentBulletCount, 0, MagazineClip);
                uiVisualer.UpdateAmmoCounter(_currentBulletCount, MagazineClip);
                return true;
            }

            return false;

        }

        private TrailRenderer _bulletTrail;

        public override void PrimaryFire()
        {
            _localFireRate += Time.deltaTime;
            _localFireRate = Mathf.Clamp(_localFireRate, 0, FireRate);


            if (_localFireRate.Equals(FireRate) && !_currentBulletCount.Equals(0))
            {
                var transform = playerCamera.transform;
                var position = transform.position;
                var forward = transform.forward;
                if (_bulletTrail)
                {
                    _bulletTrail.Clear();
                    _bulletTrail.transform.position = Vector3.zero;
                }

                _bulletTrail = _bulletTrail
                    ? _bulletTrail
                    : Object.Instantiate(Resources.Load<TrailRenderer>("WeaponEffects/Bullet Trail"));
                _bulletTrail.transform.position = visualiser.WeaponBarrel.position;

                RaycastHit hit = Hitscan(position, forward).GetClosestHit();
                if (hit.collider)
                {
                    hit.collider.GetComponent<IDamageable>()?.TakeDamage(10);
                    _bulletTrail.AddPosition(hit.point);
                }
                else
                {
                    _bulletTrail.AddPosition(forward * 100f);
                }


                Debug.DrawRay(position, forward * hit.distance, Color.red);
                _localFireRate = 0;
                _currentBulletCount--;
                uiVisualer.UpdateAmmoCounter(_currentBulletCount, MagazineClip);
            }
        }

        public override void SecondaryFire()
        {
            throw new System.NotImplementedException();
        }

        public TestingWeapon(WeaponVisualiser visualiser, CinemachineFreeLook camera, HudManager uiVisualiser) :
            base(visualiser, camera, uiVisualiser)
        {
            _currentBulletCount = MagazineClip;
            visualiser.SetWeaponModel("testWeaponModel",
                Object.Instantiate(Resources.Load<GameObject>("WeaponModels/Test_Pistol")));

            uiVisualiser.SetWeaponIcon(Resources.Load<Sprite>("WeaponIcons/Test_Pistol"));
            uiVisualiser.UpdateAmmoCounter(_currentBulletCount, MagazineClip);
        }
    }
}