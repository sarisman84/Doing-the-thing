using Player;
using Player.Weapons;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class TestWeaponAmmo : BasePickup
    {
        public override bool OnPickup(FirstPersonController controller)
        {
            if (controller.weaponController.currentWeapon is TestingWeapon weapon)
                return weapon.AddAmmo(100);
            return false;
        }
    }
}