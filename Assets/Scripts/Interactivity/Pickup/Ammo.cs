using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class Ammo : MonoBehaviour, IPickup
    {
        public void OnPickup(GameObject obj)
        {
            int result = CanBePickedUp(obj, out var weapon);
            if (result == 0) return;
            int addedAmmo = weapon.AddAmmo(AmmoType.pickupAmmount);
            gameObject.SetActive(false);
            HeadsUpDisplay.DisplayPickupMessage(obj, $"{addedAmmo}x {AmmoType.name}");
            HeadsUpDisplay.UpdateWeaponAmmoUI(obj, weapon);
        }

        public int CanBePickedUp(GameObject obj, out Weapon foundWeapon)
        {
            WeaponController playerInfo = obj.GetComponent<WeaponController>();
            foundWeapon = null;
            if (!playerInfo || !AmmoType) return 0;
            foundWeapon = playerInfo.weaponLibrary.Find(w => w.ammoType == AmmoType);
            if (!foundWeapon) return 0;
            if (foundWeapon.currentAmunition >= foundWeapon.maxAmmo) return 0;
            return 200;
        }

        public AmmoType AmmoType { get; set; }
    }
}