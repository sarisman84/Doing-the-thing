using System;
using Interactivity.Pickup;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class AmmoCrate : Crate
    {
        public AmmoType type;
        public int amountOfAmmo = 1;


        protected override void OnDeathEvent()
        {
            BasePickup.SpawnAmmo(transform, type, amountOfAmmo);
        }
    }
}