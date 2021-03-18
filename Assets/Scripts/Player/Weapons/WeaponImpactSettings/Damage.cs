using Interactivity;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem.WeaponImpactSettings
{
    [CreateAssetMenu(menuName = "Weapons/Interaction Type/Damage", fileName = "New Damage Effect", order = 0)]
    public class Damage : InteractionType
    {
        public float damage;
        public override int ApplyImpactEffectToEntity(Collider collider, GameObject owner)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(owner,damage);
                return 200;
            }

            return 199;
        }
    }
}