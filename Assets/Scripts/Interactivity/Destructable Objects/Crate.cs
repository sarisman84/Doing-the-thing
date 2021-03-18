using Interactivity.Pickup;
using Player;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class Crate : BaseCrate
    {
        [SerializeField] private int minAmountCurrency, maxAmountCurrency;
        protected override void OnDeathEvent(Collider other)
        {
            Pickup.Pickup.SpawnCurrency(transform, minAmountCurrency, maxAmountCurrency);
        }
    }
}