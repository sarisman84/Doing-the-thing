using Interactivity;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    
    public abstract class ImpactDefinition : ScriptableObject
    {
        public abstract string OnImpactCallback(Collider collider);
    }

    
    
}