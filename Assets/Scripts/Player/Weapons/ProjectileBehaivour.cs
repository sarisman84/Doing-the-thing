using System;
using System.Collections;
using Extensions;
using UnityEngine;

namespace Player.Weapons
{
    public delegate void OnCollisionEnterEvent(Transform transform, float radius, float damage);

    public class ProjectileBehaivour : MonoBehaviour
    {
        public float ProjectileDamage { set; private get; }
    
        public float CollisionRadious { set; private get; }
        public event OnCollisionEnterEvent OnHitEvent;

        public float ProjectileLifespan { set; private get; } = 5f;
        private float _lifeSpan = 0;
        private void Update()
        {
            _lifeSpan += Time.deltaTime;
            _lifeSpan = Mathf.Clamp(_lifeSpan, 0, ProjectileLifespan);
            
            if(_lifeSpan.Equals(ProjectileLifespan)) gameObject.SetActive(false);
        }


        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.Equals(null) || other.collider.GetComponent<PlayerController>()) return;
            OnHitEvent?.Invoke(transform, CollisionRadious, ProjectileDamage);
        }

        private void OnDisable()
        {
            _lifeSpan = 0;
            OnHitEvent = null;
        }
    }
}