using Interactivity;
using Player.Weapons.NewWeaponSystem.WeaponImpactSettings;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem
{
    public abstract class TargetSelectionSettings : ScriptableObject
    {
        [Expose] public ImpactSettings impactType;
        public abstract int TargetSelectionOnImpact(Collider collider);
    }
}