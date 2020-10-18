using UnityEngine;

namespace Player
{
    public class CurrencyHandler
    {
        public int Currency { get; private set; }


        public void Pay(int amount)
        {
            Currency -= amount;
        }

        public void Earn(int amount)
        {
            Currency += amount;
        }
    }
}