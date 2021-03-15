using Interactivity.Components.General;
using UnityEngine;
using Utility.Attributes;

namespace Player.Weapons.NewWeaponSystem.WeaponImpactSettings
{
    [CreateAssetMenu(menuName = "Weapons/Interaction Type/Polymorph", fileName = "New Polymorph Effect", order = 0)]
    public class Polymorph : InteractionType
    {
        public GameObject polymorphModel;
        public bool isPolymorphPassive;
        [Expose] public InteractionType extraInteraction;

        public override int ApplyImpactEffectToEntity(Collider collider)
        {
            PolymorphicEntity polymorphicEntity = collider.GetComponent<PolymorphicEntity>();
            if (polymorphicEntity)
            {
                polymorphicEntity.PolymorphEntity(polymorphModel);
                return 200;
            }

            if (extraInteraction)
                extraInteraction.ApplyImpactEffectToEntity(collider);


            return 0;
        }
    }
}