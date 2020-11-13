using System.Collections;
using System.Collections.Generic;
using Extensions;
using Interactivity.Pickup;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactivity.Enemies
{
    public class Zombie : BaseEnemy
    {
        private float _defaultSpeed;
        private Color _originalColor;
        MeshRenderer _modelRenderer;
        private MeshFilter _modelFilter;
        private Mesh _originalMesh;
        private Material _originalMaterial;
        public int maxHealth = 100;

        private float _currentHealth;

        protected override void Awake()
        {
            base.Awake();
            _currentHealth = maxHealth;
            _modelRenderer = GetComponent<MeshRenderer>();
            _modelFilter = GetComponent<MeshFilter>();

            var material = _modelRenderer.material;

            _originalColor = material.color;
            _originalMesh = _modelFilter.mesh;
            _originalMaterial = material;

            _defaultSpeed = agent.speed;
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
            if (!HasTransformed)
                agent.destination = EnemyBehaivourManager.GetCurrentTargetPosition();
            if (!agent.speed.Equals(_defaultSpeed))
                agent.speed = _defaultSpeed;
            _modelRenderer.material.color = Color.Lerp(_modelRenderer.material.color, _originalColor, 0.01f);
        }


        Dictionary<int, GameObject> _transformationList = new Dictionary<int, GameObject>();

        public override void Transform(GameObject newModel)
        {
            if (HasTransformed) return;

            if (!_transformationList.ContainsKey(newModel.GetInstanceID()))
            {
                GameObject transformation = Instantiate(newModel);
                _transformationList.Add(newModel.GetInstanceID(), transformation);
                transform.SetGameObjectAsChild(transformation);
            }
            else
            {
                _transformationList.ApplyAction(t => t.Value.SetActive(false));

                _transformationList[newModel.GetInstanceID()].SetActive(true);
            }

            _modelRenderer.enabled = false;
            _delayedDeath = StartCoroutine(DelayedDeath());
            HasTransformed = true;
        }

        private Coroutine _delayedDeath;


        IEnumerator DelayedDeath()
        {
            int time = 0;
            while (time < 10)
            {
                agent.speed = Random.Range(_defaultSpeed / 2f, _defaultSpeed);
                agent.destination = transform.position.GetRandomPositionInRange(4);
                yield return new WaitForSeconds(6f);
                time++;
            }

            yield return null;
            TakeDamage(maxHealth);
        }


        protected override void OnDeath()
        {
            gameObject.SetActive(false);
            BasePickup.SpawnCurrency(transform, 2, 14);
            _modelRenderer.enabled = true;
            _transformationList.ApplyAction(t => t.Value.SetActive(false));
            if (_delayedDeath != null)
            {
                StopCoroutine(_delayedDeath);
                _delayedDeath = null;
            }
        }
    }
}