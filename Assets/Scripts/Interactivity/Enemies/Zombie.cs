using System;
using Extensions;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Random = System.Random;

namespace Interactivity.Enemies
{
    public class Zombie : BaseEnemy
    {
        private Color _originalColor;
        MeshRenderer _modelRenderer;
        public int maxHealth = 100;

        private float _currentHealth;

        protected override void Awake()
        {
            base.Awake();
            _currentHealth = maxHealth;
            _modelRenderer = GetComponent<MeshRenderer>();
            _originalColor = _modelRenderer.material.color;
        }

        public override void TakeDamage(float damage)
        {
            _modelRenderer.sharedMaterial.color = Color.red;
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                OnDeath();

                //Reset health
                _currentHealth = maxHealth;
            }
        }

        protected override void Update()
        {
            agent.destination = EnemyBehaivourManager.Access.GetTargetPosition();
            _modelRenderer.material.color = Color.Lerp(_modelRenderer.material.color, _originalColor, 0.01f);
        }


        protected override void OnDeath()
        {
            base.OnDeath();
            Random rnd = new Random();
            for (int i = 0; i < rnd.Next(2,10); i++)
            {
                GameObject obj = ObjectManager.DynamicInstantiate(Resources.Load<GameObject>("Drops/Currency"));
                obj.SetActive(true);
                obj.transform.position = transform.position.GetRandomPositionInRange(1);
            }
            
        }
    }
}