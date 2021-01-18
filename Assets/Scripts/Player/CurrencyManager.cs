using UnityEngine;

namespace Player
{
    public class CurrencyManager : MonoBehaviour
    {
        public int Currency { get; private set; }


        public void Pay(int amount)
        {
            Currency -= amount;
            // _manager.CurrencyCounter = Currency;
            //HeadsUpDisplay.UpdateCurrencyUI(Currency);
        }

        public void Earn(int amount)
        {
            Currency += amount;
            // _manager.CurrencyCounter = Currency;
            //HeadsUpDisplay.UpdateCurrencyUI(Currency);
        }
    }
}