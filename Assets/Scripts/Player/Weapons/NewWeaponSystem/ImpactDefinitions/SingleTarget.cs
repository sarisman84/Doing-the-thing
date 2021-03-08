using Interactivity;
using Player.Weapons.NewWeaponSystem.WeaponImpactSettings;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.ImpactDefinitions
{
    [CreateAssetMenu(fileName = "New SingleTarget Settings", menuName = "Weapons/Impact Target/Single", order = 0)]
    public class SingleTarget : TargetSelectionSettings
    {
      
        public override int TargetSelectionOnImpact(Collider collider)
        {
            return impactType.ApplyImpactEffectToEntity(collider);
        }
    }
}