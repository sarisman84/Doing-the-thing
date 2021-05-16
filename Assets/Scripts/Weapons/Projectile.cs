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
        public event Action<Projectile> ONEnableEvent;
        public event Action<Projectile> ONSinglePhysicsEvent;

        private bool isSinglePhysicsEventAlreadyTriggered = false;

        #region Reset methods

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

        public void ResetEnableEvent()
        {
            ONEnableEvent = null;
        }

        public void ResetSinglePhysicsEvent()
        {
            ONSinglePhysicsEvent = null;
        }
        
        public void ResetAllEvents()
        {
            ResetCollisionEvent();
            ResetUpdateEvent();
            ResetFixedUpdateEvent();
            ResetEnableEvent();
            ResetSinglePhysicsEvent();
        }

        #endregion

        private void OnEnable()
        {
            ONEnableEvent?.Invoke(this);
            isSinglePhysicsEventAlreadyTriggered = false;
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
            if (!isSinglePhysicsEventAlreadyTriggered)
            {
                ONSinglePhysicsEvent?.Invoke(this);
                isSinglePhysicsEventAlreadyTriggered = true;
            }
            ONFixedUpdateEvent?.Invoke(this);
        }

        private void Update()
        {
            ONUpdateEvent?.Invoke(this);
        }


      
    }
}