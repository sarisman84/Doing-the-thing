using Interactivity.Pickup;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class AmmoCrate : Crate
    {
        public string ammoType = "Pickup";
        public int amountOfAmmo = 1;
        
        
        public override void TakeDamage(float damage)
        {
            OnDeath();
            BasePickup.SpawnAmmo(transform, ammoType, amountOfAmmo);
            
        }
    }
}