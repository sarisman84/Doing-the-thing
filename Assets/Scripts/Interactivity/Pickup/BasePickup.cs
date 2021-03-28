using System;
using System.Collections.Generic;
using Extensions;
using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Random = System.Random;

namespace Interactivity.Pickup
{
    public interface IPickup : IUnity
    {
        void OnPickup(GameObject obj);
        int CanBePickedUp(GameObject obj, out Weapon foundWeapon);
    }

    public static class Pickup
    {
        public static void SpawnCurrency(Transform owner, int minAmount, int maxAmount)
        {
            Random rnd = new Random();
            for (int i = 0; i < rnd.Next(minAmount, maxAmount); i++)
            {
                GameObject obj = ObjectManager.DynamicInstantiate(Resources.Load<GameObject>("Drops/Currency"));
                obj.SetActive(true);
                obj.transform.position = owner.position.GetRandomPositionInRange(2);
            }
        }

        public static void SpawnAmmoOfType(AmmoType type, Vector3 spawnPos, int amount)
        {
            float pushForce = 300;
            Random rnd = new Random();
            Ammo ammo = type.InstantiateAmmo() as Ammo;
            ammo.gameObject.SetActive(true);
            ammo.AmmoType = type;
            ammo.transform.position = spawnPos;
            ammo.gameObject.GetComponent<Rigidbody>()
                .AddForce(new Vector3(rnd.Next(-1, 1) * pushForce, 1 * (pushForce / 2f), rnd.Next(-1, 1) * pushForce));
            VentorTitleRotator rotator = ammo.gameObject.GetComponent<VentorTitleRotator>();
            if (!rotator)
            {
                rotator = ammo.gameObject.AddComponent<VentorTitleRotator>();
            }

            if (rotator)
                rotator.rotationSpeed = (float) rnd.NextDouble();
        }

        public static void SpawnRandomAmmoType(List<Weapon> weaponLibrary, Vector3 spawnPos, int amount)
        {
            Random rnd = new Random();
            SpawnAmmoOfType(weaponLibrary[rnd.Next(0, weaponLibrary.Count)].ammoType, spawnPos, amount);
        }
    }
}