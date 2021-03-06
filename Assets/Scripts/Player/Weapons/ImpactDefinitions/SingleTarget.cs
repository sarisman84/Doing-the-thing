﻿using Interactivity;
using Player.Weapons.NewWeaponSystem.WeaponImpactSettings;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.ImpactDefinitions
{
    [CreateAssetMenu(fileName = "New SingleTarget Settings", menuName = "Weapons/Target Type/Single", order = 0)]
    public class SingleTarget : TargetSelectionSettings
    {
      
        public override int TargetSelectionOnImpact(Collider collider, GameObject owner)
        {
            
            return impactType.ApplyImpactEffectToEntity(collider, owner);
        }
    }
}