using System;
using System.Collections;
using System.Globalization;
using Extensions;
using Interactivity.Events;
using Interactivity.Pickup;
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
        public TMP_Text interactionMessage;
        public TMP_Text pickupMessage;
        public PlayerController playerController;

        private int _currentAmount;
        private InteractionController _interactionController;
        private float _resetTimer;

        private static CustomEvent _ammoUIUpdateEvent;
        private static CustomEvent _currencyUIUpdateEvent;
        private static CustomEvent _weaponIconUIUpdateEvent;
        private static CustomEvent _displayPickupMessageEvent;


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

        public static void DisplayPickupMessage(GameObject owner, string message)
        {
            if (_displayPickupMessageEvent)
                _displayPickupMessageEvent.OnInvokeEvent(owner, message);
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

            _displayPickupMessageEvent =
                CustomEvent.CreateEvent<Action<string>>(PickupMessageEvent, playerController.gameObject);


            _interactionController = InteractionController.GetInteractionController(playerController.gameObject);
            if (_interactionController)
            {
                _interactionController.ONInteractionExitEvent += ResetInteractionMessage;
                _interactionController.ONInteractionEnterEvent += DisplayInteractionMessage;
            }
        }


        private void OnDisable()
        {
            _weaponIconUIUpdateEvent.RemoveEvent<Action<Sprite>>(SetWeaponIcon);
            _ammoUIUpdateEvent.RemoveEvent<Action<Weapon>>(_UpdateAmmoCounter);
            _currencyUIUpdateEvent.RemoveEvent<Action<int>>(_UpdateCurrency);
            _displayPickupMessageEvent.RemoveEvent<Action<string>>(PickupMessageEvent);

            if (_interactionController)
                _interactionController.ONInteractionExitEvent -= ResetInteractionMessage;
            _interactionController.ONInteractionEnterEvent -= DisplayInteractionMessage;
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

        private void DisplayInteractionMessage(RaycastHit obj)
        {
            Debug.Log("Is being called");
            if (interactionMessage)
            {
                interactionMessage.gameObject.SetActive(true);
                interactionMessage.text = $"Press E to interact with {obj.collider.gameObject.name}.";
            }
        }


        private void ResetInteractionMessage(RaycastHit obj)
        {
            if (interactionMessage)
            {
                interactionMessage.text = "";
                interactionMessage.gameObject.SetActive(false);
            }
        }

        private void PickupMessageEvent(string message)
        {
            if (pickupMessage)
            {
                pickupMessage.gameObject.SetActive(true);
                pickupMessage.text = $"Picked up {message}.";
                _resetTimer = 0;
            }
        }

        private void ResetPickupMessage()
        {
            if (pickupMessage)
            {
                pickupMessage.text = "";
                pickupMessage.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            _resetTimer += Time.deltaTime;
            if (_resetTimer >= 3)
            {
                ResetPickupMessage();
                _resetTimer = 0;
            }
        }
    }
}