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
    public class HudManager
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;
        public TMP_Text currencyCounter;
        public const string UpdateCurrency = "UI_HUD_UpdateCurrency";
        public const string UpdateWeaponIcon = "UI_HUD_UpdateWeaponIcon";
        public const string UpdateAmmoCounter = "UI_HUD_UpdateAmmoCounter";

        private Dictionary<string, Coroutine> _delayDisable;
        private MonoBehaviour _unityReference;

        public HudManager(MonoBehaviour monoBehaviour)
        {
            _unityReference = monoBehaviour;
            ammoCounter = GameObject.FindGameObjectWithTag("HUD/Ammo Counter").GetComponent<TMP_Text>();
            weaponIcon = GameObject.FindGameObjectWithTag("HUD/Weapon Icon").GetComponent<Image>();
            currencyCounter = GameObject.FindGameObjectWithTag("HUD/Currency Counter").GetComponent<TMP_Text>();
            currencyCounter.text = "";
            ammoCounter.transform.parent.gameObject.SetActive(false);
            currencyCounter.transform.parent.gameObject.SetActive(false);
            EventManager.AddListener<Action<int>>(UpdateCurrency, _UpdateCurrency);
            EventManager.AddListener<Action<Weapon>>(UpdateAmmoCounter, _UpdateAmmoCounter);
            EventManager.AddListener<Action<Sprite>>(UpdateWeaponIcon, SetWeaponIcon);

            _delayDisable = new Dictionary<string, Coroutine>();
        }

        public float CurrencyCounter
        {
            set => currencyCounter.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void _UpdateCurrency(int amount)
        {
            if (!_delayDisable.Equals(null))
            {
                if (_delayDisable.ContainsKey("UpdateCurrency"))
                    _unityReference.StopCoroutine(_delayDisable["UpdateCurrency"]);
            }

            GameObject gameObject;
            (gameObject = currencyCounter.transform.parent.gameObject).SetActive(true);
            CurrencyCounter = amount;
            if (_delayDisable.ContainsKey("UpdateCurrency"))
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
            else
            {
                _delayDisable.Add("UpdateCurrency", _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f)));
            }
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

            if (_delayDisable != null)
            {
                if (_delayDisable.ContainsKey("UpdateAmmo"))
                    _unityReference.StopCoroutine(_delayDisable["UpdateAmmo"]);
            }

            GameObject gameObject;
            (gameObject = ammoCounter.transform.parent.gameObject).SetActive(true);
            ammoCounter.text = $"{weapon.maxAmmoCount}/{weapon.currentAmmoCount}";
            if (_delayDisable != null && _delayDisable.ContainsKey("UpdateAmmo"))
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
            else
            {
                _delayDisable.Add("UpdateAmmo", _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f)));
            }
        }

        private void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            if (_delayDisable != null)
            {
                if (_delayDisable.ContainsKey("UpdateIcon"))
                    _unityReference.StopCoroutine(_delayDisable["UpdateIcon"]);
            }

            GameObject gameObject;
            (gameObject = ammoCounter.transform.parent.gameObject).SetActive(true);
            weaponIcon.sprite = icon;
            if (_delayDisable != null && _delayDisable.ContainsKey("UpdateIcon"))
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
            else
            {
                _delayDisable.Add("UpdateIcon", _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f)));
            }
        }
    }
}