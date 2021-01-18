using System;
using Interactivity.Components;
using Interactivity.Pickup;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    [RequireComponent(typeof(DamageableEntity))]
    public class Crate : MonoBehaviour
    {
        [SerializeField] private int minAmountCurrency, maxAmountCurrency;
        protected DamageableEntity damageableEntity;

        private void Awake()
        {
            damageableEntity = GetComponent<DamageableEntity>();
            damageableEntity.maxHealth = 1;
            damageableEntity.onDeathEvent.AddListener(OnDeathEvent);
        }


        protected virtual void OnDeathEvent()
        {
            BasePickup.SpawnCurrency(transform, minAmountCurrency, maxAmountCurrency);
        }
    }
}