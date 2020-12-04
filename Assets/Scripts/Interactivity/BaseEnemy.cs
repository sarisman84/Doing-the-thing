using System;
using Extensions;
using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Interactivity
{
    public class BaseEnemy : MonoBehaviour, IDamageable, IPolymorphable
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

        public bool IsDead => gameObject.activeSelf;


        protected virtual void OnDeath()
        {
            gameObject.SetActive(false);
        }


        public virtual void Transform(GameObject newModel)
        {
        }

        public virtual bool HasTransformed { get; set; }
    }
}