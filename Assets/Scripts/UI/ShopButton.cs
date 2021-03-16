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
                _image = _image ? _image : transform.GetChild(0).GetComponent<Image>();
                _image.sprite = value;
            }
        }

        private Image _image;


        public void BuyItem(GameObject owner)
        {
            WeaponController controller = owner.GetComponent<WeaponController>();
            if (!controller) return;
            int priceToPay = _sellAssignedWeaponAmmo
                ? (int) (AssignedWeapon.ammoType.price * (AssignedWeapon.maxAmmo - AssignedWeapon.currentAmunition))
                : AssignedWeapon.price;

            if (CurrencyHandler.GetCurrency(owner) >= priceToPay)
            {
                CurrencyHandler.PayCurrency(owner, priceToPay);
                if (!_sellAssignedWeaponAmmo)
                {
                    controller.AddWeaponToLibrary(AssignedWeapon);
                }
                else
                {
                    controller.ResupplyWeapon(AssignedWeapon);
                }
            }


           
            
        }

        public void UpdateButton(List<Weapon> incomingLibrary)
        {
            _sellAssignedWeaponAmmo = incomingLibrary.Contains(AssignedWeapon);
            DisplayIcon();
        }


        public string DisplayPrice()
        {
            return _sellAssignedWeaponAmmo
                ? $"{AssignedWeapon.ammoType.price.ToString()} per ammo (Total Price: {(int) (AssignedWeapon.ammoType.price * (AssignedWeapon.maxAmmo - AssignedWeapon.currentAmunition))})"
                : AssignedWeapon.price.ToString();
        }

        public string DisplayTitle()
        {
            return _sellAssignedWeaponAmmo ? AssignedWeapon.ammoType.name : AssignedWeapon.name;
        }

        public string DisplayDecription()
        {
            return _sellAssignedWeaponAmmo
                ? $"Ammunition for the {AssignedWeapon.name}."
                : AssignedWeapon.description;
        }

        public void DisplayItem()
        {
            if (_sellAssignedWeaponAmmo)
            {
                if (AmmoModel)
                    AmmoModel.SetActive(true);
            }
            else if (WeaponModel)
                WeaponModel.SetActive(true);
        }

        public void DisplayIcon()
        {
            ShopIcon = AssignedWeapon.weaponIcon;
        }

        public void ResetDisplay()
        {
            if (AmmoModel)
                AmmoModel.SetActive(false);
            if (WeaponModel)
                WeaponModel.SetActive(false);
        }
    }
}