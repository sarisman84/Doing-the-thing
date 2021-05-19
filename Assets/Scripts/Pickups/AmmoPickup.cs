using System;
using Player;
using Scripts.Weapons;
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


        private void OnDrawGizmos()
        {
            Collider collider = GetComponent<Collider>();
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + new Vector3(0, 0.25f, 0), collider.bounds.size.x / 2f);
        }
    }
}