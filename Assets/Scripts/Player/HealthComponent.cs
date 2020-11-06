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

        private void Die()
        {
            gameObject.SetActive(false);
        }

        public void OnFall()
        {
            //EventManager.TriggerEvent("PlayerFall_DetachFromPlayer");
            EventManager.TriggerEvent(PlayerController.CameraFallBehaivourEvent, CameraBehaivour.Look);
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, true);
            
            _physics.AddForce(transform.forward * 1600f, ForceMode.Acceleration);
            StartCoroutine(Respawn(4f, LatestRespawnPos));
        }

        IEnumerator Respawn(float respawnTime, Vector3 spawnPos)
        {
            float time = 0;
            while (time < respawnTime)
            {
                yield return new WaitForEndOfFrame();
                EventManager.TriggerEvent(PlayerController.ConstantlyLookTowardsThePlayerEvent);
                time += Time.deltaTime;
            }

            yield return null;
            transform.position = spawnPos;
            _physics.velocity = Vector3.zero;
            // EventManager.TriggerEvent("Player_DisableCursor");
            // EventManager.TriggerEvent("PlayerFall_FollowPlayer");
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, false);
            EventManager.TriggerEvent(PlayerController.CameraFallBehaivourEvent, CameraBehaivour.Follow);
           

            yield return null;
        }


        public enum CameraBehaivour
        {
            Follow,
            Look,
            FollowAndLook
        }

        public static object SetCameraBehaivour(CinemachineFreeLook camera, Transform target, CameraBehaivour value)
        {
            switch (value)
            {
                case CameraBehaivour.Follow:
                    camera.Follow = target;
                    camera.LookAt = null;
                    camera.transform.parent = target;
                    break;
                case CameraBehaivour.Look:
                    camera.Follow = null;
                    camera.LookAt = target;
                    camera.transform.parent = null;
                    break;
                case CameraBehaivour.FollowAndLook:
                    camera.Follow = target;
                    camera.LookAt = target;
                    camera.transform.parent = target;
                    break;
            }

            return null;
        }

        public static object RotateCameraTowards(CinemachineFreeLook camera, Transform target)
        {
            var cameraTransform = camera.transform;
            camera.transform.rotation = Quaternion.Lerp(cameraTransform.rotation,
                Quaternion.LookRotation(target.position - cameraTransform.position, Vector3.up), 0.25f);
            return null;
        }
    }
}