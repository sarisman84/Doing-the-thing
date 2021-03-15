using System;
using System.Collections.Generic;
using Player;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopButton : CustomUIElement
    {
        public Weapon AssignedWeapon { get; set; }
        private bool _sellAssignedWeaponAmmo;

        public GameObject WeaponModel { get; set; }
        public GameObject AmmoModel { get; set; }

        public Sprite ShopIcon
        {
            set
            {
                _image = _image != null ? _image : transform.GetChild(0).GetComponent<Image>();
                _image.sprite = value;
            }
        }

        private Image _image;


        public void BuyItem(GameObject owner)
        {
            WeaponController controller = owner.GetComponent<WeaponController>();
            if (!controller) return;
            int priceToPay = _sellAssignedWeaponAmmo ? AssignedWeapon.ammoType.price : AssignedWeapon.price;

            if (CurrencyHandler.GetCurrency(owner) >= priceToPay)
            {
                CurrencyHandler.PayCurrency(owner, priceToPay);
            }


            if (!_sellAssignedWeaponAmmo)
            {
                controller.AddWeaponToLibrary(AssignedWeapon);
            }
            else
            {
                controller.ResupplyWeapon(AssignedWeapon);
            }
        }

        public void UpdateButton(List<Weapon> incomingLibrary)
        {
            _sellAssignedWeaponAmmo = incomingLibrary.Contains(AssignedWeapon);
            if (_sellAssignedWeaponAmmo)
                AmmoModel.SetActive(true);
            else
                WeaponModel.SetActive(true);
        }


        public string DisplayPrice()
        {
            return _sellAssignedWeaponAmmo ? AssignedWeapon.ammoType.price.ToString() : AssignedWeapon.price.ToString();
        }

        public string DisplayTitle()
        {
            return _sellAssignedWeaponAmmo ? AssignedWeapon.ammoType.name : AssignedWeapon.name;
        }

        public string DisplayDecription()
        {
            return _sellAssignedWeaponAmmo
                ? $"Ammunition for the {AssignedWeapon.name} weapon."
                : AssignedWeapon.description;
        }
    }
}