using System;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Pickup;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using static Player.InputListener.KeyCode;

namespace Player
{
    public class InteractionController : MonoBehaviour
    {
        [Space] public LayerMask interactionFilter;
        public float detectionRange = 20, interactionRange = 10;
        private PlayerController _player;
        private WeaponController _weaponController;
        private float _lastInteractionDistance = 0;
        public bool showPickupRange = false;


        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _weaponController = GetComponent<WeaponController>();
        }


        private void Update()
        {
            InteractWithEntities(detectionRange, interactionFilter);
        }

        public bool _isInteracting { get; set; }

        private void InteractWithEntities(float detectionRadius, LayerMask interactionMask)
        {
            Collider[] foundObjs = Physics.OverlapSphere(transform.position, detectionRadius, interactionMask);

            if (foundObjs.Equals(null)) return;
            foreach (var t in foundObjs)
            {
                Weapon weapon = _weaponController != null ? _weaponController.currentWeapon : null;
                TriggerProximity(t);
                if (TriggerInteraction(t, weapon)) continue;


                t.gameObject.SetActive(false);
            }
        }

        private void TriggerProximity(Collider collider1)
        {
            IInteractable interactable = collider1.GetComponent<IInteractable>();
            if (interactable == null) return;


            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance >= _lastInteractionDistance && _lastInteractionDistance != 0)
            {
                interactable.OnProximityExit();
                _lastInteractionDistance = 0;
            }

            if (distance >= interactionRange) return;
            if (_lastInteractionDistance == 0 && CanBeInteracted(interactable))
            {
                _lastInteractionDistance = distance;
                interactable.OnProximityEnter();
            }
        }

        private bool TriggerInteraction(Collider entity, object args = null)
        {
            if (entity.Equals(null)) return false;
            foreach (var component in entity.GetComponents<Component>())
            {
                switch (component)
                {
                    case BasePickup pickup:
                        if (pickup && args != null)
                        {
                            return !pickup.OnPickup((Weapon) args);
                        }

                        return true;

                    case IInteractable interactable:
                        float distance = Vector3.Distance(transform.position, interactable.transform.position);

                        if (distance >= interactionRange || !CanBeInteracted(interactable)) return true;
                        switch (interactable.InputType)
                        {
                            case InteractionInput.Hold:
                                if (InputListener.GetKey(Interact))
                                    interactable.OnInteract(_player);
                                break;
                            case InteractionInput.Press:
                                if (InputListener.GetKeyDown(Interact))
                                    interactable.OnInteract(_player);
                                break;
                        }


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

        private bool CanBeInteracted(IInteractable interactable)
        {
            float dotProduct;
            if (interactable.NeedToLookAtInteractable)
            {
                Vector3 direction = interactable.transform.position - _player.transform.position;
                direction.Normalize();

                dotProduct = Vector3.Dot(direction, _player.playerCamera.transform.forward.normalized);
                return dotProduct >= 0.9f;
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if (!showPickupRange) return;
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, interactionRange);
        }
    }
}