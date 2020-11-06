using Player.Weapons;
using UnityEngine;
using Utility;

namespace Player
{
    public class CurrencyHandler
    {
        
        public const string EarnCurrency = "Currency_Earn";
        public const string PayCurrency = "Currency_Pay";
        public CurrencyHandler()
        {
            EventManager.AddListener(EarnCurrency, o => Earn((int)o));
            EventManager.AddListener(PayCurrency, o => Pay((int)o));
            EventManager.TriggerEvent(HudManager.UpdateCurrency, Currency);
            
        }
        
        public int Currency { get; private set; }


        private object Pay(int amount)
        {
            Currency -= amount;
            // _manager.CurrencyCounter = Currency;
            EventManager.TriggerEvent(HudManager.UpdateCurrency, Currency);
            return null;
        }

        private object Earn(int amount)
        {
            Currency += amount;
            // _manager.CurrencyCounter = Currency;
            EventManager.TriggerEvent(HudManager.UpdateCurrency, Currency);
            return null;
        }
    }
}