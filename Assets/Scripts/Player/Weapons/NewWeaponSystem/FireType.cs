using System;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem
{
    public abstract class FireType : ScriptableObject
    {
        
        [Expose] public TargetSelectionSettings targetSelectionType;
        public abstract void Fire(Vector3 origin, Vector3 direction);
    }

   

   
}