using System;
using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public Rigidbody physics;
        public event Action<Collision> ONCollisionEvent;
        public event Action ONFixedUpdateEvent;
        public event Action ONUpdateEvent;
        private void Awake()
        {
            physics = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            ONCollisionEvent?.Invoke(other);
        }

        private void FixedUpdate()
        {
            ONFixedUpdateEvent?.Invoke();
        }

        private void Update()
        {
            ONUpdateEvent?.Invoke();
        }
    }
}