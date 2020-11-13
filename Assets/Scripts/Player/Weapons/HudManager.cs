using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Player.Weapons
{
    public class HudManager
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;
        public TMP_Text currencyCounter;
        public const string UpdateCurrency = "UI_UpdateCurrency";


      
        public HudManager()
        {
            ammoCounter = GameObject.FindGameObjectWithTag("HUD/Ammo Counter").GetComponent<TMP_Text>();
            weaponIcon = GameObject.FindGameObjectWithTag("HUD/Weapon Icon").GetComponent<Image>();
            currencyCounter = GameObject.FindGameObjectWithTag("HUD/Currency Counter").GetComponent<TMP_Text>();
            EventManager.AddListener(UpdateCurrency, new Action<int>(_UpdateCurrency));
        }

        public float CurrencyCounter
        {
            set => currencyCounter.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void _UpdateCurrency(int amount)
        {
            CurrencyCounter = amount;
        }

        public void UpdateAmmoCounter(Weapon weapon)
        {
            ammoCounter.text = $"{weapon.maxAmmoCount}/{weapon.currentAmmoCount}";
        }

        public void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            weaponIcon.sprite = icon;
        }
    }
}