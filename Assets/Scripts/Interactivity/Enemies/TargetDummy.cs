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
            damageableEntity.onDamageTakenEvent.AddListener(TakeDamage);
            originalColor = modelRenderer.material.color;
        }

        public void TakeDamage(int currentHealth)
        {
            modelRenderer.sharedMaterial.color = Color.red;
        }

        void Update()
        {
            modelRenderer.material.color = Color.Lerp(modelRenderer.material.color, originalColor, 0.01f);
        }
    }
}