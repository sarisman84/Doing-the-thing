using System;
using System.Collections;
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

        private Coroutine _delayDisable;
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
            EventManager.AddListener(UpdateCurrency, new Action<int>(_UpdateCurrency));
        }

        public float CurrencyCounter
        {
            set => currencyCounter.text = value.ToString(CultureInfo.InvariantCulture);
        }

        private void _UpdateCurrency(int amount)
        {
            if (!_delayDisable.Equals(null))
            {
                _unityReference.StopCoroutine(_delayDisable);
            }

            GameObject gameObject;
            (gameObject = currencyCounter.transform.parent.gameObject).SetActive(true);
            CurrencyCounter = amount;
            _delayDisable =
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
        }

        private IEnumerator DisableOnDelay(GameObject parentGameObject, float f)
        {
            yield return new WaitForSeconds(f);
            parentGameObject.SetActive(false);
        }

        public void UpdateAmmoCounter(Weapon weapon)
        {
            if (weapon.maxAmmoCount == -1)
            {
                ammoCounter.text = "";
                return;
            }

            if (_delayDisable != null)
            {
                _unityReference.StopCoroutine(_delayDisable);
            }

            GameObject gameObject;
            (gameObject = ammoCounter.transform.parent.gameObject).SetActive(true);
            ammoCounter.text = $"{weapon.maxAmmoCount}/{weapon.currentAmmoCount}";
            _delayDisable =
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
        }

        public void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            if (_delayDisable != null)
            {
                _unityReference.StopCoroutine(_delayDisable);
            }

            GameObject gameObject;
            (gameObject = ammoCounter.transform.parent.gameObject).SetActive(true);
            weaponIcon.sprite = icon;
            _delayDisable =
                _unityReference.StartCoroutine(DisableOnDelay(gameObject, 3f));
        }
    }
}