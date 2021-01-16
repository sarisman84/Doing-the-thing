using System;
using Interactivity.Events;
using Player.Weapons;
using UnityEngine;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace Player
{
    public class CurrencyHandler : MonoBehaviour
    {
        private static CustomEvent _getCurrencyEvent;
        private static CustomEvent _payCurrencyEvent;
        private static CustomEvent _earnCurrencyEvent;
        public PlayerController playerController;

        private void Awake()
        {
            _getCurrencyEvent =
                CustomEvent.CreateEvent<Func<int>>(ref _getCurrencyEvent, () => Currency, playerController.gameObject);
            _payCurrencyEvent =
                CustomEvent.CreateEvent<Action<int>>(ref _payCurrencyEvent, Pay, playerController.gameObject);
            _earnCurrencyEvent =
                CustomEvent.CreateEvent<Action<int>>(ref _earnCurrencyEvent, Earn, playerController.gameObject);
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
            HeadsUpDisplay.UpdateCurrencyUI(gameObject, Currency);
        }

        private void Earn(int amount)
        {
            Currency += amount;
            // _manager.CurrencyCounter = Currency;
            HeadsUpDisplay.UpdateCurrencyUI(gameObject, Currency);
        }
    }
}