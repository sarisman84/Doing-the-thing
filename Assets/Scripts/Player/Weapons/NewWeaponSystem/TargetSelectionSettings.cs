using Interactivity;
using Player.Weapons.NewWeaponSystem.WeaponImpactSettings;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem
{
    public abstract class TargetSelectionSettings : ScriptableObject
    {
        [Expose] public ImpactEffect impactEffect;
        [Expose] public DamageEffect impactType;
        public abstract int TargetSelectionOnImpact(Collider collider);
    }
}