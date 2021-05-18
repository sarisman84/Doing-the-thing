using System;
using Player.Scripts;
using UnityEngine;

namespace Scripts
{
    public class AmmoPickup : Pickup
    {
        public string weaponID;

        private void Start()
        {
            PickupManager.RegisterPickup(this);
            GameObject refModel = WeaponLibrary.GlobalWeaponLibrary[weaponID].AmmoPickupModel;
            if (refModel)
            {
                GameObject model = Instantiate(refModel, transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
            }
        }

        public override void OnPickup(PickupInteractor interactor)
        {
            WeaponHandler handler = interactor.GetComponent<WeaponHandler>();
            if (handler && handler.AddAmmoToWeapon(weaponID))
            {
                this.SetActive(false);
            }
        }
    }
}