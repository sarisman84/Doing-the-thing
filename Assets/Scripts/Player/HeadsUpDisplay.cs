using System;
using System.Collections;
using System.Globalization;
using Interactivity.Events;
using Player.Weapons.NewWeaponSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Action = Interactivity.Enemies.Finite_Statemachine.Action;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace Player
{
    public class HeadsUpDisplay : MonoBehaviour
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;
        public TMP_Text currencyCounter;
        public TMP_Text interactionHeadsUpDisplay;
        public PlayerController playerController;

        private int _currentAmount;
        private InteractionController _interactionController;

        private static CustomEvent _ammoUIUpdateEvent;
        private static CustomEvent _currencyUIUpdateEvent;
        private static CustomEvent _weaponIconUIUpdateEvent;


        #region Static Methods

        public static void UpdateWeaponAmmoUI(GameObject owner, Weapon weapon)
        {
            if (_ammoUIUpdateEvent)
                _ammoUIUpdateEvent.OnInvokeEvent(owner, weapon);
        }

        public static void UpdateWeaponIconUI(GameObject owner, Sprite currentWeaponIcon)
        {
            if (_weaponIconUIUpdateEvent)
                _weaponIconUIUpdateEvent.OnInvokeEvent(owner, currentWeaponIcon);
        }

        public static void UpdateCurrencyUI(GameObject owner, int currency)
        {
            if (_currencyUIUpdateEvent)
                _currencyUIUpdateEvent.OnInvokeEvent(owner, currency);
        }

        #endregion

        private void OnEnable()
        {
            _weaponIconUIUpdateEvent =
                CustomEvent.CreateEvent<Action<Sprite>>(SetWeaponIcon,
                    playerController.gameObject);
            _ammoUIUpdateEvent = CustomEvent.CreateEvent<Action<Weapon>>(_UpdateAmmoCounter,
                playerController.gameObject);
            _currencyUIUpdateEvent = CustomEvent.CreateEvent<Action<int>>(_UpdateCurrency,
                playerController.gameObject);


            _interactionController = InteractionController.GetInteractionController(playerController.gameObject);
            if (_interactionController)
            {
                _interactionController.ONInteractionExitEvent += ResetInteractionHud;
                _interactionController.ONInteractionEnterEvent += DisplayInteractionHud;
            }
        }


        private void OnDisable()
        {
            _weaponIconUIUpdateEvent.RemoveEvent<Action<Sprite>>(SetWeaponIcon);
            _ammoUIUpdateEvent.RemoveEvent<Action<Weapon>>(_UpdateAmmoCounter);
            _currencyUIUpdateEvent.RemoveEvent<Action<int>>(_UpdateCurrency);

            if (_interactionController)
                _interactionController.ONInteractionExitEvent -= ResetInteractionHud;
                _interactionController.ONInteractionEnterEvent -= DisplayInteractionHud;
        }

        private void Awake()
        {
            _UpdateCurrency(CurrencyHandler.GetCurrency(playerController.gameObject));
        }


        public float CurrencyCounter
        {
            set { currencyCounter.text = value.ToString(CultureInfo.InvariantCulture); }
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
            if (weapon.maxAmmo == -1)
            {
                ammoCounter.text = "";
                return;
            }

            ammoCounter.text = $"{weapon.maxAmmo}/{weapon.currentAmunition}";
        }

        private void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            weaponIcon.sprite = icon;
        }

        private void DisplayInteractionHud(RaycastHit obj)
        {
            Debug.Log("Is being called");
            if (interactionHeadsUpDisplay)
            {
                interactionHeadsUpDisplay.gameObject.SetActive(true);
                interactionHeadsUpDisplay.text = $"Press E to interact with {obj.collider.gameObject.name}.";
            }
        }


        private void ResetInteractionHud(RaycastHit obj)
        {
            if (interactionHeadsUpDisplay)
            {
                interactionHeadsUpDisplay.text = "";
                interactionHeadsUpDisplay.gameObject.SetActive(false);
            }
        }
    }
}