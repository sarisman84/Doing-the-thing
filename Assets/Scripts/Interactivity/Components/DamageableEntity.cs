using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Interactivity.Components
{
    public class DamageableEntity : MonoBehaviour, IDamageable
    {
        public float maxHealth;
        private float _currentHealth;

        [Space] public UnityEvent<Collider> onDamageTakenEvent;
        public UnityEvent<Collider> onDeathEvent;


        private void OnEnable()
        {
            _currentHealth = maxHealth;
        }


        public void TakeDamage(Collider col, float damage)
        {
            onDamageTakenEvent?.Invoke(col);
            _currentHealth -= damage;
            _currentHealth = !_currentHealth.Equals(-1) ? Mathf.Clamp(_currentHealth, 0, maxHealth) : _currentHealth;
            if (_currentHealth.Equals(0))
            {
                OnDeath(col);
            }
        }

        public void OnDeath(Collider col)
        {
            onDeathEvent?.Invoke(col);
            gameObject.SetActive(false);
        }


       
    }
}