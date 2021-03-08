using System;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    public abstract class FireDefinition : ScriptableObject
    {
        public abstract void FireCustomEffect(Vector3 origin, Vector3 direction,
            Func<Collider, string> onImpactCallback);
    }

   

   
}