using System;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Components;
using Interactivity.Events;
using Interactivity.Pickup;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;
using Utility.Attributes;
using static Player.InputListener.KeyCode;

namespace Player
{
    public class InteractionController : MonoBehaviour
    {
        [Space] public LayerMask interactionFilter;
        public LayerMask pickupFilter;
        public float detectionRange = 20, interactionRange = 20;
        private PlayerController _player;
        private WeaponController _weaponController;
        private float _lastInteractionDistance = 0;
        
       

      

      

        public bool showPickupRange = false;
        private Collider _col;
        private CameraController _cameraController;

#if UNITY_EDITOR
        [EnableIf("showPickupRange")] [SerializeField]
        private Color interactionColor, detectionColor;
#endif
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _weaponController = GetComponent<WeaponController>();
            _cameraController = GetComponent<CameraController>();
            _col = GetComponent<Collider>();
        }


        private void Update()
        {
            DetectEntityFromRaycast(interactionRange);
            DetectEntitiesInProximity(detectionRange);
        }

        private void DetectEntitiesInProximity(float detectionRadius)
        {
            Collider[] foundObjs = Physics.OverlapSphere(transform.position, detectionRadius, pickupFilter);

            if (foundObjs.Equals(null)) return;
            foreach (var t in foundObjs)
            {
                Weapon weapon = _weaponController != null ? _weaponController.currentWeapon : null;
                if (OnTriggerDetection(t, weapon)) continue;

                

                t.gameObject.SetActive(false);
            }
        }

        private void DetectEntityFromRaycast(float rayLength)
        {
            RaycastHit closestHit;
            Ray ray = new Ray(transform.position,
                _cameraController == null ? transform.forward : _cameraController.PlayerCamera.forward);
            if (Physics.Raycast(ray, out closestHit, rayLength, interactionFilter))
            {
                if (closestHit.collider == null) return;
                IInteractable interactable = closestHit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                    switch (interactable.InputType)
                    {
                        case InteractionInput.Hold:
                            if (InputListener.GetKey(Interact)) interactable.OnInteract(_col);
                            break;
                        case InteractionInput.Press:
                            if (InputListener.GetKeyDown(Interact)) interactable.OnInteract(_col);
                            break;
                    }
            }
        }


        private void ProximityCheck(Vector3 target, Action onEnter, Action onStay, Action onExit, float range)
        {
            float distance = Vector3.Distance(transform.position, target);
            if (distance >= interactionRange)
            {
                if (distance >= _lastInteractionDistance && _lastInteractionDistance != 0)
                {
                    onExit?.Invoke();
                    _lastInteractionDistance = 0;
                }
            }
            else if (_lastInteractionDistance == 0)
            {
                _lastInteractionDistance = distance;
                onEnter?.Invoke();
            }

            if (distance < interactionRange)
            {
                onStay?.Invoke();
            }
        }

        private bool OnTriggerDetection(Collider entity, object args = null)
        {
            if (entity.Equals(null)) return false;
            foreach (var component in entity.GetComponents<Component>())
            {
                switch (component)
                {
                    case BasePickup pickup:
                        if (pickup && args != null)
                        {
                            bool result = pickup.OnPickup((Weapon) args);
                           

                            return !result;
                        }

                        return true;

                    case IDetectable interactable:
                        
                        var position = interactable.transform.position;
                        ProximityCheck(position, () => interactable.OnAreaEnter(_col),
                            () => interactable.OnAreaStay(_col), () => interactable.OnAreaExit(_col), detectionRange);


                        return true;
                    default:
                        if (entity.CompareTag("Currency"))
                        {
                            EventManager.TriggerEvent(CurrencyHandler.EarnCurrency, 1);
                            return false;
                        }

                        continue;
                }
            }


            return true;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showPickupRange) return;
            Gizmos.color = detectionColor - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, detectionRange);

            Gizmos.color = interactionColor - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, interactionRange);
        }

#endif
    }
}