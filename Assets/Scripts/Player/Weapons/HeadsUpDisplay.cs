using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Player.Weapons
{
    public class HeadsUpDisplay : MonoBehaviour
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;
        public TMP_Text currencyCounter;
        private const string UpdateCurrency = "UI/HUD/UpdateCurrency";
        private const string UpdateWeaponIcon = "UI/HUD/UpdateWeaponIcon";
        private const string UpdateAmmoCounter = "UI/HUD/UpdateAmmoCounter";


        private void Awake()
        {
            currencyCounter.text = "";
        }


        public static void UpdateCurrencyUI(int amm)
        {
            EventManager.TriggerEvent(UpdateCurrency, amm);
        }

        public static void UpdateAmmoUI(Weapon curWeapon)
        {
            EventManager.TriggerEvent(UpdateAmmoCounter, curWeapon);
        }

        public static void UpdateWeaponIconUI(Sprite value)
        {
            EventManager.TriggerEvent(UpdateWeaponIcon, value);
        }

        private void OnEnable()
        {
            EventManager.AddListener<Action<int>>(UpdateCurrency, _UpdateCurrency);
            EventManager.AddListener<Action<Weapon>>(UpdateAmmoCounter, _UpdateAmmoCounter);
            EventManager.AddListener<Action<Sprite>>(UpdateWeaponIcon, SetWeaponIcon);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<Action<int>>(UpdateCurrency, _UpdateCurrency);
            EventManager.RemoveListener<Action<Weapon>>(UpdateAmmoCounter, _UpdateAmmoCounter);
            EventManager.RemoveListener<Action<Sprite>>(UpdateWeaponIcon, SetWeaponIcon);
        }

        public float CurrencyCounter
        {
            set => currencyCounter.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void _UpdateCurrency(int amount)
        {
            CurrencyCounter = amount;
        }

        private IEnumerator DisableOnDelay(GameObject parentGameObject, float f)
        {
            yield return new WaitForSeconds(f);
            parentGameObject.SetActive(false);
        }

        private void _UpdateAmmoCounter(Weapon weapon)
        {
            if (weapon.maxAmmoCount == -1)
            {
                ammoCounter.text = "";
                return;
            }
            ammoCounter.text = $"{weapon.maxAmmoCount}/{weapon.currentAmmoCount}";
        }

        private void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            weaponIcon.sprite = icon;
        }
    }
}