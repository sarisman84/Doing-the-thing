using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Player.Weapons.NewWeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileBehaviour : MonoBehaviour
    {
        private Rigidbody _physicsController;
        private float _velocity, _lifeSpan;

        private float _currentLifespan;
        private bool _disableObject;

        private Func<Collider, int> _onImpactCallback;
        private Dictionary<int, GameObject> currentModels = new Dictionary<int, GameObject>();

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
            
            _disableObject = !_disableObject && _currentLifespan >= _lifeSpan;
        }

        private void OnCollisionCheck()
        {
            Collider[] foundColliders = Physics.OverlapSphere(transform.position, 0.1f);
            if (foundColliders != null)
            {
                Collider other = foundColliders.First();
                if (other.gameObject.GetComponent<PlayerController>() != null)
                {
                    _onImpactCallback?.Invoke(other);
                    _disableObject = true;
                }
                
            }
        }

        private void FixedUpdate()
        {
            _physicsController.velocity = transform.forward.normalized * (_velocity * 100f * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            _onImpactCallback?.Invoke(other.collider);
            _disableObject = true;
        }

        public void UpdateInformation(Vector3 origin, Vector3 direction, float newVelocity, float newLifeSpan,
            Func<Collider, int> onImpactCallback)
        {
            transform.position = origin;
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            _velocity = newVelocity;
            _lifeSpan = newLifeSpan;


            _onImpactCallback = onImpactCallback;
        }

        public void UpdateProjectileModel(GameObject projectileModel)
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