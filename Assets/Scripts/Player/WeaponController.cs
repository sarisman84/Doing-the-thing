using System;
using System.Collections.Generic;
using Extensions.InputExtension;
using Player.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

namespace Player
{
    public class WeaponController
    {
        [SerializeReference] private BaseWeapon _currentWeapon;


        private InputActionReference _aimSightsInput, _primaryFireInput, _secondaryFireInput;
        private FirstPersonController _controller;

        public WeaponController(InputActionReference aimSights, InputActionReference primaryFire,
            InputActionReference secondaryFire, FirstPersonController controller)
        {
            _aimSightsInput = aimSights;
            _primaryFireInput = primaryFire;
            _secondaryFireInput = secondaryFire;
            _controller = controller;

            controller.onUpdateCallback += LocalUpdate;

            _currentWeapon =
                new TestingWeapon(controller.playerCamera.transform.GetComponentInChildren<WeaponVisualiser>());
        }


        void LocalUpdate()
        {
            if (_currentWeapon == null) return;

            AimToCursor(_currentWeapon.visualiser.transform);

            if (_primaryFireInput.GetInputValue<bool>())
            {
                _currentWeapon.PrimaryFire();
            }
        }

        private void AimToCursor(Transform transform)
        {
            var playerCameraTransform = _controller.playerCamera.transform;
            var position = playerCameraTransform.position;
            var forward = playerCameraTransform.forward;
            Ray ray = new Ray(position - transform.right * 0.13f, forward);


            RaycastHit[] hit = new RaycastHit[5];

            Physics.RaycastNonAlloc(ray, hit);
            Debug.DrawRay(ray.origin, ray.direction * hit[0].distance, Color.yellow);
            RaycastHit closestHit = hit[0];

            float minDistance = float.MaxValue;
            for (int i = 0; i < hit.Length; i++)
            {
                if (minDistance > hit[i].distance)
                {
                    if (hit[i].collider)
                        if (hit[i].collider.GetComponent<FirstPersonController>())
                            continue;
                    if (hit[i].distance == 0) continue;
                    minDistance = hit[i].distance;
                    closestHit = hit[i];
                }
            }

            //Debug.Log($"{closestHit.distance}/{hit.Length}");

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(
                    closestHit.collider ? (closestHit.point - transform.position).normalized : forward,
                    Vector3.up), 0.4f);
        }
    }
}