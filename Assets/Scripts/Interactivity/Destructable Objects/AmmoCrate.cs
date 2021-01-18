using System;
using Interactivity.Pickup;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class AmmoCrate : Crate
    {
        public string ammoType = "Pickup";
        public int amountOfAmmo = 1;


        protected override void OnDeathEvent()
        {
            BasePickup.SpawnAmmo(transform, ammoType, amountOfAmmo);
        }
    }
}