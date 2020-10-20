using Player.Weapons;
using UnityEngine;

namespace Player
{
    public class CurrencyHandler
    {
        private HudManager _manager;

        public CurrencyHandler(FirstPersonController controller)
        {
            _manager = controller.transform.GetComponentInChildren<HudManager>();
            _manager.CurrencyCounter = Currency;
        }
        
        public int Currency { get; private set; }


        public void Pay(int amount)
        {
            Currency -= amount;
            _manager.CurrencyCounter = Currency;
        }

        public void Earn(int amount)
        {
            Currency += amount;
            _manager.CurrencyCounter = Currency;
        }
    }
}