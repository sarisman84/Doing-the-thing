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

        [Space] public UnityEvent<int> onDamageTakenEvent;
        public UnityEvent onDeathEvent;
        
        
        public int CurrentHealth => Mathf.RoundToInt(_currentHealth);


        private void OnEnable()
        {
            _currentHealth = maxHealth;
        }


        public void TakeDamage(Collider col, float damage)
        {
            onDamageTakenEvent?.Invoke(CurrentHealth);
            _currentHealth -= _currentHealth.Equals(-1) ? 0 : damage;
            _currentHealth = !_currentHealth.Equals(-1) ? Mathf.Clamp(_currentHealth, 0, maxHealth) : _currentHealth;
            if (_currentHealth.Equals(0))
            {
                OnDeath(col);
            }
        }

        public void OnDeath(Collider col)
        {
            onDeathEvent?.Invoke();
            gameObject.SetActive(false);
        }
    }
}