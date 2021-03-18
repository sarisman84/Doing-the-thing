using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class Ammo : MonoBehaviour, IPickup
    {
        public int OnPickup(GameObject obj)
        {
            WeaponController playerInfo = obj.GetComponent<WeaponController>();
            Debug.Log($"Attempting to pickup ammo! -> {playerInfo}");
            if (!playerInfo || !ammoType) return 0;
            Debug.Log(ammoType);
            Weapon weapon = playerInfo.weaponLibrary.Find(w => w.ammoType == ammoType);
            Debug.Log(
                $"{(weapon ? $"Has found weapon: {weapon.name}" : $"Couldn't find any weapon within {playerInfo.gameObject.name}'s library.")}");
            if (!weapon) return 0;
            int result = weapon.AddAmmo(ammoType.pickupAmmount);
            if (result != 201)
                gameObject.SetActive(false);
            return result;
        }

        public AmmoType ammoType { get; set; }
    }
}