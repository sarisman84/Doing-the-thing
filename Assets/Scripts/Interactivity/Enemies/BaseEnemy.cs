using System;
using Extensions;
using Interactivity.Components;
using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Interactivity
{
    [RequireComponent(typeof(DamageableEntity))]
    public abstract class BaseEnemy : MonoBehaviour, IPolymorphable
    {
        protected NavMeshAgent agent;
        protected DamageableEntity damageableEntity;


        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            damageableEntity = GetComponent<DamageableEntity>();
        }

        protected abstract void Update();
        public abstract void Transform(GameObject newModel);
        public virtual bool HasTransformed { get; set; }
    }
}