using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
   
    public abstract class InteractionType : ScriptableObject
    {
        public abstract int ApplyImpactEffectToEntity(Collider collider, GameObject gameObject);
    }

    
}