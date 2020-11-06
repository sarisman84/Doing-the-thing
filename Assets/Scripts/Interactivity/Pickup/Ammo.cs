using Player;
using Player.Weapons;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class Ammo : BasePickup
    {
        public string ammoType = "Test_Pistol";
        public override bool OnPickup(Weapon currentWeapon)
        {
            if (currentWeapon.ID.Equals(WeaponManager.globalWeaponLibrary[ammoType].ID))
                return currentWeapon.AddAmmo(100);
            return false;
        }
    }
}