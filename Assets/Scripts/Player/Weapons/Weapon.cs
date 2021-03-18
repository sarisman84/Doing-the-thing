using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactivity.Pickup;
using JetBrains.Annotations;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/New Weapon", fileName = "New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public int price = 1;
        public string description = "Insert description here";
        [Space] public float maxAmmo;
        [Expose] public AmmoType ammoType;
        [Space] public float fireRate;
        [Space] [SerializeField] private GameObject weaponModelPrefab;
        public Sprite weaponIcon;
        [Space] [Expose] public FireType fireType;


        #region Static Methods

        public static Weapon GetWeaponViaName(GameObject owner, string name)
        {
            Weapon foundWeapon = Resources.Load<Weapon>($"Weapons/{name}");
            if (foundWeapon && owner)
                foundWeapon.Setup(owner);
            return foundWeapon;
        }

        public static List<Weapon> GetAllWeapons(GameObject owner = null)
        {
            List<Weapon> results = Resources.LoadAll<Weapon>("Weapons/").ToList();
            if (results != null && owner)
            {
                results.ApplyAction(w => w.Setup(owner));
            }

            return results;
        }

        #endregion


        GameObject _currentOwner;
        private float _tempFireRate;
        private float currentAmmo;

        public float currentAmunition => currentAmmo;

        public GameObject Owner => _currentOwner;
        public GameObject WeaponModelPrefab => weaponModelPrefab;
        public GameObject InstancedWeaponModel { get; set; }

        public void Setup(GameObject owner)
        {
            if (owner)
            {
                _currentOwner = owner;
                currentAmmo = maxAmmo;

                if (ammoType && ammoType.ammoPrefab)
                {
                    Ammo pickup = ammoType.ammoPrefab.GetComponent<Ammo>();
                    pickup.ammoType = ammoType;
                }

                return;
            }

            throw new NullReferenceException($"Owner is null: {owner}");
        }


        public int TriggerFire(bool input)
        {
            _tempFireRate += Time.deltaTime;
            if (input && _tempFireRate >= fireRate && currentAmmo > 0)
            {
                _tempFireRate = 0;
                OnFire(InstancedWeaponModel.transform.GetChild(0));
                currentAmmo--;
                HeadsUpDisplay.UpdateWeaponAmmoUI(Owner, this);
                return 200;
            }

            return 199;
        }


        void OnFire(Transform barrel)
        {
            fireType.Fire(barrel.transform.position,
                barrel.forward.normalized, Owner);
        }

        public int AddAmmo(int amount)
        {
            if (Math.Abs(currentAmmo - maxAmmo) < 0.01f) return 201;
            Debug.Log($"Adding ammo for {name}");
            currentAmmo += amount;
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
            HeadsUpDisplay.UpdateWeaponAmmoUI(Owner, this);
            return 200;
        }
    }


    public static class WeaponExtensions
    {
        public static Weapon SwapWeaponTo(List<Weapon> weaponLibrary,
            [NotNull] Weapon newWeapon)
        {
            // if (originalWeapon == null) throw new ArgumentNullException(nameof(originalWeapon));


            HeadsUpDisplay.UpdateWeaponIconUI(newWeapon.Owner, newWeapon.weaponIcon);
            HeadsUpDisplay.UpdateWeaponAmmoUI(newWeapon.Owner, newWeapon);
            WeaponVisualiser.UpdateWeaponModel(newWeapon.Owner, weaponLibrary, newWeapon);

            return newWeapon;
        }
    }
}