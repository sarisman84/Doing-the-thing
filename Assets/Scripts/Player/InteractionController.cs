using System;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Pickup;
using Player.Weapons;
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
        public int currentCurrency = 0;
        private PlayerController _player;


        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }


        private void Update()
        {
            _isInteracting = InputListener.GetKeyDown(Interact);
            InteractWithEntities(detectionRange, interactionFilter);
        }

        public bool _isInteracting { get; set; }

        private void InteractWithEntities(float detectionRadius, LayerMask interactionMask)
        {
            Collider[] foundObjs = Physics.OverlapSphere(transform.position, detectionRadius, interactionMask);

            if (foundObjs.Equals(null)) return;
            foreach (var t in foundObjs)
            {
                if (TriggerInteraction(t, _player.weaponController.currentWeapon)) continue;


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
                        if (pickup)
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
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, interactionRange);
        }
    }
}