using System;
using Player.Weapons;
using UnityEngine;
using Utility;

namespace Player
{
    public class CurrencyHandler
    {
        public const string EarnCurrency = "Currency_Earn";
        public const string PayCurrency = "Currency_Pay";
        public const string GetCurrency = "Currency_GetInfo";


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameLoad()
        {
            new CurrencyHandler();
        }

        public CurrencyHandler()
        {
            EventManager.AddListener<Action<int>>(EarnCurrency, Earn);
            EventManager.AddListener<Action<int>>(PayCurrency, Pay);
            EventManager.AddListener<Func<int>>(GetCurrency, () => Currency);
            // EventManager.TriggerEvent(HeadsUpDisplay.UpdateCurrency, Currency);
            HeadsUpDisplay.UpdateCurrencyUI(Currency);
        }

        public int Currency { get; private set; }


        private void Pay(int amount)
        {
            Currency -= amount;
            // _manager.CurrencyCounter = Currency;
            HeadsUpDisplay.UpdateCurrencyUI(Currency);
        }

        private void Earn(int amount)
        {
            Currency += amount;
            // _manager.CurrencyCounter = Currency;
            HeadsUpDisplay.UpdateCurrencyUI(Currency);
        }
    }
}