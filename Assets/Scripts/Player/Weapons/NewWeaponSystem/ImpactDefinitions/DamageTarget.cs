using Interactivity;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.ImpactDefinitions
{
    [CreateAssetMenu(fileName = "New Damage Settings", menuName = "Weapons/Impact Effect/Damage Entity", order = 0)]
    public class DamageTarget : ImpactDefinition
    {
        public float damage;
        public override string OnImpactCallback(Collider collider)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                return $"Damage dealt! Damage amm - {damage}";
            }

            return "Didnt detect any target";
        }
    }
}