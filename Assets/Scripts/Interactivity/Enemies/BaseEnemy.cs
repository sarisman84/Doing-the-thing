using System;
using Extensions;
using Interactivity.Components;
using Interactivity.Components.General;
using Interactivity.Pickup;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Interactivity
{
    [RequireComponent(typeof(DamageableEntity))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        protected NavMeshAgent agent;
        protected DamageableEntity damageableEntity;
        protected PolymorphicEntity polymorphicEntity;


        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            damageableEntity = GetComponent<DamageableEntity>() ?? gameObject.AddComponent<DamageableEntity>();
            polymorphicEntity = GetComponent<PolymorphicEntity>() ?? gameObject.AddComponent<PolymorphicEntity>();
        }


    }
}