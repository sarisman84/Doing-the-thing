using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player.Weapons.NewWeaponSystem.FireDefinitions;
using Player.Weapons.NewWeaponSystem.ImpactDefinitions;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBehaviour : MonoBehaviour
    {
        private Projectile _projectileSettings;

        private float _currentLifespan;
        private bool _disableObject;


        private Dictionary<int, GameObject> currentModels = new Dictionary<int, GameObject>();
        private Rigidbody _physicsController;


        private void OnEnable()
        {
            (_physicsController = _physicsController ? _physicsController : GetComponent<Rigidbody>()).velocity =
                Vector3.zero;
            _physicsController.angularVelocity = Vector3.zero;
            _currentLifespan = 0;
        }

        private void Update()
        {
            _currentLifespan += Time.deltaTime;


            // OnCollisionCheck();

            if (_disableObject)
                gameObject.SetActive(false);

            _disableObject = !_disableObject && _currentLifespan >= _projectileSettings.projectileLifespan;
        }


        private void FixedUpdate()
        {
            _physicsController.velocity = transform.forward.normalized *
                                          (_projectileSettings.projectileVelocity * 100f * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            _projectileSettings.targetSelectionType.TargetSelectionOnImpact(other.collider);
            var detectionRange = (_projectileSettings.targetSelectionType as MultiTarget)?.detectionRange;

            CoroutineManager.Instance.StartCoroutine(
                _projectileSettings.targetSelectionType.impactEffect.PlayImpactEffect(other.GetContact(0).point,
                    (-transform.forward) - other.GetContact(0).normal,
                    detectionRange ?? 1));
            _disableObject = true;
        }

        public void UpdateInformation(Vector3 origin, Vector3 direction, Projectile definition)
        {
            transform.position = origin;
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            _projectileSettings = definition;

            UpdateProjectileModel(definition.projectileModel);
        }

        private void UpdateProjectileModel(GameObject projectileModel)
        {
            //Reset any previous models.
            currentModels.ApplyAction(p => p.Value.SetActive(false));
            //Create an instance of the model if it does not exist.
            if (!currentModels.ContainsKey(projectileModel.GetInstanceID()))
            {
                currentModels.Add(projectileModel.GetInstanceID(),
                    Instantiate(projectileModel, transform));
            }

            //Use said instance of the model and enable it.
            currentModels.FirstOrDefault(p => p.Key.Equals(projectileModel.GetInstanceID())).Value.SetActive(true);
        }
    }
}