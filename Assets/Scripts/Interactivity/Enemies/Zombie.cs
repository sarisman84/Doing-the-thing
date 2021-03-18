using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using Interactivity.Pickup;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactivity.Enemies
{
    public class Zombie : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            polymorphicEntity.onLatePolymorph.AddListener(ResetHealth);
        }

        private void ResetHealth()
        {
            damageableEntity.HealEntity(int.MaxValue);
            
        }

        
    }
}