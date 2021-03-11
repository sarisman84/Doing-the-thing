using Interactivity;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.WeaponImpactSettings
{
    [CreateAssetMenu(menuName = "Weapons/Damage Effects/Default", fileName = "New Damage Effect", order = 0)]
    public class Damage : DamageEffect
    {
        public float damage;
        public override int ApplyImpactEffectToEntity(Collider collider)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                return 200;
            }

            return 199;
        }
    }
}