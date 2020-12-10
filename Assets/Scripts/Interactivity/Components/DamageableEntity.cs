using System;
using System.Collections;
using Interactivity.Events;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Attributes;

namespace Interactivity.Components
{
    public class DamageableEntity : MonoBehaviour, IDamageable
    {
        public float maxHealth;
        private float _currentHealth;

        [Space] public UnityEvent onDamageTakenEvent;
        public UnityEvent onDeathEvent;

        public bool useCustomEvent;

        [EnableIf("useCustomEvent")] public CustomEventWrapper onDamageTakenEventWrapper, onDeathEventWrapper;


        private void OnEnable()
        {
            _currentHealth = maxHealth;
        }


        public void TakeDamage(Collider col, float damage)
        {
            onDamageTakenEvent?.Invoke();
            if (!onDamageTakenEventWrapper.customEvent)
                onDamageTakenEventWrapper.customEvent.Raise(onDamageTakenEventWrapper.triggerOnce);
            _currentHealth -= damage;
            _currentHealth = !_currentHealth.Equals(-1) ? Mathf.Clamp(_currentHealth, 0, maxHealth) : _currentHealth;
            if (_currentHealth.Equals(0))
            {
                OnDeath(col);
            }
        }

        public void OnDeath(Collider col)
        {
            onDeathEvent?.Invoke();
            if (!onDeathEventWrapper.customEvent) onDeathEventWrapper.customEvent.Raise(false);
            gameObject.SetActive(false);
        }
    }
}