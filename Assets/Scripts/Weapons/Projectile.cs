using System;
using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public Rigidbody physics;
        public event Action<Collision, Projectile> ONCollisionEvent;
        public event Action<Projectile> ONFixedUpdateEvent;
        public event Action<Projectile> ONUpdateEvent;



        public void ResetCollisionEvent()
        {
            ONCollisionEvent = null;
        }

        public void ResetUpdateEvent()
        {
            ONUpdateEvent = null;
        }

        public void ResetFixedUpdateEvent()
        {
            ONFixedUpdateEvent = null;
        }

        private void Awake()
        {
            physics = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            ONCollisionEvent?.Invoke(other, this);
        }

        private void FixedUpdate()
        {
            ONFixedUpdateEvent?.Invoke(this);
        }

        private void Update()
        {
            ONUpdateEvent?.Invoke(this);
        }
    }
}