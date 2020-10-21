using System;
using Extensions;
using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Interactivity
{
    public class BaseEnemy : MonoBehaviour, IDamageable
    {
        protected NavMeshAgent agent;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Update()
        {
        }

        public virtual void TakeDamage(float damage)
        {
            OnDeath();
        }


        protected virtual void OnDeath()
        {
            gameObject.SetActive(false);
        }

        
    }
}