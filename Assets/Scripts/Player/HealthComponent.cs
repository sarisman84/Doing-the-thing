using System;
using System.Collections;
using Cinemachine;
using Interactivity;
using UnityEngine;
using Utility;

namespace Player
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        public float maxHealth;
        private float _currentHealth;


        private Rigidbody _physics;

        public Vector3 LatestRespawnPos { private get; set; } = Vector3.zero;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _physics = GetComponent<Rigidbody>();
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            if (_currentHealth.Equals(0))
            {
                Die();
            }
        }

        public bool IsDead => gameObject.activeSelf;
        public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

        private void Die()
        {
            gameObject.SetActive(false);
        }

        public void OnFall()
        {
            //EventManager.TriggerEvent("PlayerFall_DetachFromPlayer");
            EventManager.TriggerEvent(CameraController.CameraFallBehaivourEvent, CameraController.CameraBehaivour.Look);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, true);
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            _physics.AddForce(transform.forward * 1600f, ForceMode.Acceleration);
            StartCoroutine(Respawn(4f, LatestRespawnPos));
        }

        IEnumerator Respawn(float respawnTime, Vector3 spawnPos)
        {
            float time = 0;
            while (time < respawnTime)
            {
                yield return new WaitForEndOfFrame();
                EventManager.TriggerEvent(CameraController.ConstantlyLookTowardsThePlayerEvent);
                time += Time.deltaTime;
            }

            yield return null;
            transform.position = spawnPos;
            _physics.velocity = Vector3.zero;
            // EventManager.TriggerEvent("Player_DisableCursor");
            // EventManager.TriggerEvent("PlayerFall_FollowPlayer");
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, false);
            EventManager.TriggerEvent(CameraController.CameraFallBehaivourEvent, CameraController.CameraBehaivour.Follow);
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);


            yield return null;
        }


       

        

       
    }
}