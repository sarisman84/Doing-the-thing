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
            originalColor = modelRenderer.material.color;
        }

        public override void TakeDamage(float damage)
        {
            modelRenderer.sharedMaterial.color = Color.red;
            
        }

        protected override void Update()
        {
            modelRenderer.material.color = Color.Lerp(modelRenderer.material.color, originalColor, 0.01f);
        }

        
    }
}