﻿using System;
using Interactivity.Pickup;
using Player;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class AmmoCrate : BaseCrate
    {
        public int amountOfAmmo = 1;


        protected override void OnDeathEvent(GameObject attacker)
        {
            WeaponController controller = default;
            if (attacker)
                controller = attacker.GetComponent<WeaponController>();

            Pickup.Pickup.SpawnRandomAmmoType(controller ? controller.weaponLibrary : Weapon.GetAllWeapons(),
                transform.position, amountOfAmmo);
        }
    }
}