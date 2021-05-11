using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class DamageableObject : MonoBehaviour
    {
        public float maxHealth;
        private float currentHealth;

        public bool useEvent;
        public UnityEvent onDeathEvent;
        
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }


        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (useEvent)
                onDeathEvent.Invoke();
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}