using System;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.FireDefinitions
{
    [CreateAssetMenu(menuName = "Weapons/Fire Effects/Projectile", fileName = "New Projectile", order = 0)]
    public class Projectile : FireDefinition
    {
        public float projectileLifespan = 5f;
        public float projectileVelocity = 15f;
        public GameObject projectileModel;

        public override void Fire(Vector3 origin, Vector3 direction)
        {
            ProjectileBehaviour projectile =
                ObjectManager.DynamicInstantiate(Resources.Load<ProjectileBehaviour>("Weapons/Projectile"), false, 500);
            projectile.gameObject.SetActive(true);
            projectile.UpdateInformation(origin, direction, projectileVelocity, projectileLifespan, weaponImpactEffect.TargetSelectionOnImpact);
            projectile.UpdateProjectileModel(projectileModel);
        }
    }
}