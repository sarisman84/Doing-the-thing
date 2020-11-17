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
        public bool holdDownInteractionKey = true;
        public bool showPickupRange = false;


        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _weaponController = GetComponent<WeaponController>();
        }


        private void Update()
        {
            _isInteracting = holdDownInteractionKey
                ? InputListener.GetKey(Interact)
                : InputListener.GetKeyDown(Interact);
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
                if (TriggerInteraction(t, weapon)) continue;


                t.gameObject.SetActive(false);
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

                        if (distance >= interactionRange) return true;
                        if (_isInteracting)
                        {
                            interactable.OnInteract(_player);
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