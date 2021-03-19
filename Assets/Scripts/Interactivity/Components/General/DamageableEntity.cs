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
        public bool godMode = false;

        [Space] public UnityEvent<int> onUpdateEvent;
        public UnityEvent<int> onDamageTakenEvent;
        public UnityEvent<GameObject> onDeathEvent;


        public int CurrentHealth => Mathf.RoundToInt(_currentHealth);

        private void Update()
        {
            onUpdateEvent?.Invoke(CurrentHealth);
        }


        private void OnEnable()
        {
            _currentHealth = maxHealth;
        }


        public void TakeDamage(GameObject attacker, float damage)
        {
            onDamageTakenEvent?.Invoke(CurrentHealth);
            _currentHealth -= _currentHealth.Equals(-1) ? 0 : damage;
            _currentHealth = !_currentHealth.Equals(-1) ? Mathf.Clamp(_currentHealth, 0, maxHealth) : _currentHealth;
            if (_currentHealth.Equals(0))
            {
                OnDeath(attacker);
            }
        }


        public void OnDeath(GameObject attacker)
        {
            if (godMode) return;
            onDeathEvent?.Invoke(attacker);
            gameObject.SetActive(false);
        }

        public void HealEntity(int healValue)
        {
            _currentHealth = Mathf.Clamp(healValue, 0, maxHealth);
        }
    }
}