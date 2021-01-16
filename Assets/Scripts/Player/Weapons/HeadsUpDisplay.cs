using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Interactivity.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;
using Object = UnityEngine.Object;

namespace Player.Weapons
{
    public class HeadsUpDisplay : MonoBehaviour
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;
        public TMP_Text currencyCounter;
        public PlayerController playerController;

        private static CustomEvent _ammoUIUpdateEvent;
        private static CustomEvent _currencyUIUpdateEvent;
        private static CustomEvent _weaponIconUIUpdateEvent;

        public static void UpdateWeaponAmmoUI(GameObject owner, Weapon weapon)
        {
            _ammoUIUpdateEvent?.OnInvokeEvent(owner, weapon);
        }

        public static void UpdateWeaponAmmoUI(Collider owner, Weapon weapon)
        {
            _ammoUIUpdateEvent?.OnInvokeEvent(owner.gameObject, weapon);
        }

        public static void UpdateWeaponIconUI(GameObject owner, Sprite currentWeaponIcon)
        {
            _weaponIconUIUpdateEvent?.OnInvokeEvent(owner, currentWeaponIcon);
        }

        public static void UpdateCurrencyUI(GameObject owner, int currency)
        {
            _currencyUIUpdateEvent?.OnInvokeEvent(owner, currency);
        }


        private void Awake()
        {
            currencyCounter.text = "";


            _weaponIconUIUpdateEvent =
                CustomEvent.CreateEvent<Action<Sprite>>(ref _weaponIconUIUpdateEvent, SetWeaponIcon,
                    playerController.gameObject);
            _ammoUIUpdateEvent = CustomEvent.CreateEvent<Action<Weapon>>(ref _ammoUIUpdateEvent, _UpdateAmmoCounter,
                playerController.gameObject);
            _currencyUIUpdateEvent = CustomEvent.CreateEvent<Action<int>>(ref _currencyUIUpdateEvent, _UpdateCurrency,
                playerController.gameObject);
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