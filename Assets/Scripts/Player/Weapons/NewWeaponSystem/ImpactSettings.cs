using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
   
    public abstract class ImpactSettings : ScriptableObject
    {
        public abstract int ApplyImpactEffectToEntity(Collider collider);
    }

    
}