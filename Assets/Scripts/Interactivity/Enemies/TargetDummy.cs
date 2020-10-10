using System;
using UnityEngine;

namespace Interactivity.Enemies
{
    public class TargetDummy : BaseEnemies
    {
        public MeshRenderer modelRenderer;
        private Color originalColor;

        private void Awake()
        {
            originalColor = modelRenderer.sharedMaterial.color;
        }

        public override void TakeDamage(float damage)
        {
            modelRenderer.sharedMaterial.color = Color.red;
            base.TakeDamage(damage);
        }

        private void Update()
        {
            modelRenderer.sharedMaterial.color = Color.Lerp(modelRenderer.sharedMaterial.color, originalColor, 0.01f);
        }
    }
}