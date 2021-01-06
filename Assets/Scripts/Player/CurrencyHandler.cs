using System;
using Interactivity.Events;
using Player.Weapons;
using UnityEngine;
using Utility;

namespace Player
{
    public class CurrencyHandler : MonoBehaviour
    {
        private static InstanceEvent _getCurrencyEvent;
        private static InstanceEvent _payCurrencyEvent;
        private static InstanceEvent _earnCurrencyEvent;
        public PlayerController playerController;

        private void Awake()
        {
            InstanceEvent.CreateEvent<Func<int>>(ref _getCurrencyEvent, playerController.gameObject, () => Currency);
            InstanceEvent.CreateEvent<Action<int>>(ref _payCurrencyEvent, playerController.gameObject, Pay);
            InstanceEvent.CreateEvent<Action<int>>(ref _earnCurrencyEvent, playerController.gameObject, Earn);
        }


        public int Currency { get; private set; }

        public static int GetCurrency(GameObject owner)
        {
            return (int) _getCurrencyEvent?.OnInvokeEventAndGetResult(owner);
        }

        public static void PayCurrency(GameObject o, int newWeaponPrice)
        {
            _payCurrencyEvent?.OnInvokeEvent(o, newWeaponPrice);
        }

        public static void EarnCurrency(GameObject o, int value)
        {
            _earnCurrencyEvent?.OnInvokeEvent(o, value);
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