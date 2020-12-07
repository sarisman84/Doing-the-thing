using System;
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
        public float detectionRange = 20f;
        private float _currentHealth;
        private float _randomPositionCounter;
        private Vector3 _randomPosition;
        Dictionary<int, GameObject> _transformationList = new Dictionary<int, GameObject>();
        protected override void Awake()
        {
            base.Awake();
            damageableEntity.onDeathEvent.AddListener(arg0 => OnDeath());
            _modelRenderer = GetComponent<MeshRenderer>();
            _modelFilter = GetComponent<MeshFilter>();

            var material = _modelRenderer.material;

            _originalColor = material.color;
            _originalMesh = _modelFilter.mesh;
            _originalMaterial = material;

            _defaultSpeed = agent.speed;
        }
        

       
        protected override void Update()
        {
            if (!HasTransformed)
            {
                Vector3 target = EnemyBehaivourManager.GetCurrentTargetPosition();

                if (Vector3.Distance(target, transform.position) < detectionRange)
                    agent.destination = target;
                else
                {
                    agent.destination = transform.position.GetRandomPositionInRange(4, 6f, ref _randomPositionCounter, ref _randomPosition);
                }
            }
            if (!agent.speed.Equals(_defaultSpeed))
                agent.speed = _defaultSpeed;
            _modelRenderer.material.color = Color.Lerp(_modelRenderer.material.color, _originalColor, 0.01f);
        }


      

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
            damageableEntity.TakeDamage(GetComponent<Collider>(), maxHealth);
        }


        private void OnDeath()
        {
            
            BasePickup.SpawnCurrency(transform, 2, 14);
            _modelRenderer.enabled = true;
            _transformationList.ApplyAction(t => t.Value.SetActive(false));
            if (_delayedDeath != null)
            {
                StopCoroutine(_delayedDeath);
                _delayedDeath = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red - new Color(0, 0, 0, 0.75f);
            Gizmos.DrawSphere(transform.position, detectionRange);
        }
    }
}