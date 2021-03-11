using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
   
    public abstract class DamageEffect : ScriptableObject
    {
        public abstract int ApplyImpactEffectToEntity(Collider collider);
    }

    
}