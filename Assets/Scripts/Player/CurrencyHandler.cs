using System;
using Interactivity.Events;
using Player.Weapons;
using UI.HUD;
using UnityEngine;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;
using Random = UnityEngine.Random;

namespace Player
{
    public class CurrencyHandler : MonoBehaviour
    {
        private static CustomEvent _getCurrencyEvent;
        private static CustomEvent _payCurrencyEvent;
        private static CustomEvent _earnCurrencyEvent;
        public PlayerController playerController;

        private InteractionController _interactionController;
        private void Awake()
        {
            _getCurrencyEvent =
                CustomEvent.CreateEvent<Func<int>>(() => Currency, playerController.gameObject);
            _payCurrencyEvent =
                CustomEvent.CreateEvent<Action<int>>(Pay, playerController.gameObject);
            _earnCurrencyEvent =
                CustomEvent.CreateEvent<Action<int>>(Earn, playerController.gameObject);
        }

        private void OnEnable()
        {
            if (!_interactionController)
            {
                _interactionController = InteractionController.GetInteractionController(playerController.gameObject);
            }
            _interactionController.ONDetectionEnterEvent += PickupCurrency;
        }

        private void OnDisable()
        {
            _interactionController.ONDetectionEnterEvent -= PickupCurrency;
        }

        private void PickupCurrency(Collider obj)
        {
            if (obj.CompareTag("Currency"))
            {
                EarnCurrency(playerController.gameObject, Random.Range(1, 5));
                obj.gameObject.SetActive(false);
            }
        }


        public int Currency { get; private set; }

        public static int GetCurrency(GameObject owner)
        {
            if (_getCurrencyEvent != null)
                return (int) _getCurrencyEvent.OnInvokeEvent(owner, null);
            return default;
        }

        public static void PayCurrency(GameObject o, int newWeaponPrice)
        {
            if (_payCurrencyEvent != null)
                _payCurrencyEvent.OnInvokeEvent(o, newWeaponPrice);
        }

        public static void EarnCurrency(GameObject o, int value)
        {
            if (_earnCurrencyEvent != null)
                _earnCurrencyEvent.OnInvokeEvent(o, value);
        }


        private void Pay(int amount)
        {
            Currency -= amount;
            // _manager.CurrencyCounter = Currency;
            HUDManager.UpdateCurrencyUI(gameObject, Currency);
        }

        private void Earn(int amount)
        {
            Currency += amount;
            // _manager.CurrencyCounter = Currency;
            HUDManager.UpdateCurrencyUI(gameObject, Currency);
        }
    }
}