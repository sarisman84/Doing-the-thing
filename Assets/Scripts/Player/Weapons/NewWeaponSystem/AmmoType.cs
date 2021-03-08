using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    [CreateAssetMenu(fileName = "New Ammo Type", menuName = "Weapons/Ammo Type", order = 0)]
    public class AmmoType : ScriptableObject
    {
        [Header("Drop Information")]
        public BasePickup ammoPrefab;
        public int pickupAmmount;

        [Header("Shop Information")]
        public int price;
        

        public BasePickup InstantiateAmmo()
        {
            return ObjectManager.DynamicComponentInstantiate(ammoPrefab);
        }
    }
}