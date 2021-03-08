using Player.Weapons.NewWeaponSystem.WeaponImpactSettings;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem.ImpactDefinitions
{
    [CreateAssetMenu(fileName = "New MultiTarget Settings", menuName = "Weapons/Impact Target/MultiTarget", order = 0)]
    public class MultiTarget : TargetSelectionSettings
    {
        public float detectionRange = 3f;


        public override int TargetSelectionOnImpact(Collider collider)
        {
            Collider[] foundColliders = Physics.OverlapSphere(collider.transform.position, detectionRange);

            if (foundColliders != null)
            {
                int result = 199;
                foreach (var col in foundColliders)
                {
                    result = impactType.ApplyImpactEffectToEntity(col);
                }

                return result;
            }

            return 198;
        }
    }
}