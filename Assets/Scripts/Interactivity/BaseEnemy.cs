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

        protected static void DropCurrency(Transform owner, int minAmount, int maxAmount)
        {
            Random rnd = new Random();
            for (int i = 0; i < rnd.Next(minAmount, maxAmount); i++)
            {
                GameObject obj = ObjectManager.DynamicInstantiate(Resources.Load<GameObject>("Drops/Currency"));
                obj.SetActive(true);
                obj.transform.position = owner.position.GetRandomPositionInRange(2);
            }
        }

        protected static void DropAmmo(Transform owner, string type, int amount)
        {
            BasePickup ammo = ObjectManager.DynamicComponentInstantiate(Resources.Load<BasePickup>($"Drops/{type}"));
            ammo.gameObject.SetActive(true);
            ammo.transform.position = owner.position;
        }
    }
}