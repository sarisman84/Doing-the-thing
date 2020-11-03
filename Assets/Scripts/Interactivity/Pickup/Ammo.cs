using Player;
using Player.Weapons;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class Ammo : BasePickup
    {
        public string ammoType = "Test_Pistol";
        public override bool OnPickup(FirstPersonController controller)
        {
            if (controller.weaponController.currentWeapon.ID.Equals(WeaponManager.globalWeaponLibrary[ammoType].ID))
                return controller.weaponController.currentWeapon.AddAmmo(100);
            return false;
        }
    }
}