using System;
using UnityEngine;

namespace Interactivity.Enemies
{
    public class TargetDummy : BaseEnemy
    {
        public MeshRenderer modelRenderer;
        private Color originalColor;

        protected override void Awake()
        {
            base.Awake();
            damageableEntity.maxHealth = -1;
            damageableEntity.onDamageTakenEvent.AddListener(args => TakeDamage());
            originalColor = modelRenderer.material.color;
        }

        public void TakeDamage()
        {
            modelRenderer.sharedMaterial.color = Color.red;
        }

        protected override void Update()
        {
            modelRenderer.material.color = Color.Lerp(modelRenderer.material.color, originalColor, 0.01f);
        }

        public override void Transform(GameObject newModel)
        {
            
        }
    }
}