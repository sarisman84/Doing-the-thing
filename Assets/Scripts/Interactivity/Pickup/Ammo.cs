using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class Ammo : BasePickup
    {
        public AmmoType ammoType;

        public override int OnPickup(Weapon currentWeapon)
        {
            if (currentWeapon.ammoType == ammoType)
                return currentWeapon.AddAmmo(ammoType.pickupAmmount);
            return 0;
        }
    }
}