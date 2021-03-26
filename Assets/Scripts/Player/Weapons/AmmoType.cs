using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    [CreateAssetMenu(fileName = "New Ammo Type", menuName = "Weapons/Ammo Type", order = 0)]
    public class AmmoType : ScriptableObject
    {
        [Header("Drop Information")] public GameObject ammoPrefab;
        public int pickupAmmount;

        [Header("Shop Information")] public bool canBePurchased;
        public int price;
        public int refillAmount;


        public IPickup InstantiateAmmo()
        {
            return ObjectManager.DynamicInstantiate(ammoPrefab).GetComponent<IPickup>();
        }
    }
}