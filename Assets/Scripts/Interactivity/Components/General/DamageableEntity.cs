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
        public UnityEvent<Collider> onDeathEvent;


        public int CurrentHealth => Mathf.RoundToInt(_currentHealth);

        private void Update()
        {
            onUpdateEvent?.Invoke(CurrentHealth);
        }


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

        public void TakeDamage(GameObject obj, float damage)
        {
            TakeDamage(gameObject.GetComponent<Collider>(), damage);
        }

        public void TakeDamage(float damage)
        {
            TakeDamage(default(GameObject),damage);
        }

        public void OnDeath(Collider col)
        {
            if (godMode) return;
            onDeathEvent?.Invoke(col);
            gameObject.SetActive(false);
        }

        public void HealEntity(int healValue)
        {
            _currentHealth = Mathf.Clamp(healValue, 0, maxHealth);
        }
    }
}