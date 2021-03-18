using System;
using Interactivity.Components;
using Interactivity.Pickup;
using UnityEngine;

namespace Interactivity.Destructable_Objects
{
    [RequireComponent(typeof(DamageableEntity))]
    public abstract class BaseCrate : MonoBehaviour
    {
        
        protected DamageableEntity damageableEntity;

        private void Awake()
        {
            damageableEntity = GetComponent<DamageableEntity>();
            damageableEntity.maxHealth = 1;
            damageableEntity.onDeathEvent.AddListener(OnDeathEvent);
        }


        protected abstract void OnDeathEvent(Collider col);
    }
}