using System;
using Interactivity.Pickup;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    public class Crate : MonoBehaviour, IDamageable
    {
        [SerializeField] private int minAmountCurrency, maxAmountCurrency;

        public virtual void TakeDamage(float damage)
        {
            OnDeath();
            BasePickup.SpawnCurrency(transform, minAmountCurrency, maxAmountCurrency);
        }

        protected void OnDeath()
        {
            gameObject.SetActive(false);
        }
    }
}