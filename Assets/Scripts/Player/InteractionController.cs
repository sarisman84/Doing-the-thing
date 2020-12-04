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
        public event Action<BasePickup> ONPickupCallback;
        public event Action<IInteractable> ONInteractionCallback;
        public event Action<IDamageable> ONKillCallback;

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
            ProximityCheck(interactable.transform, () => interactable.OnProximityEnter(),
                () => interactable.OnProximityExit(), interactionRange, _lastInteractionDistance,
                () => CanBeInteracted(interactable));

            IDamageable damageable = collider1.GetComponent<IDamageable>();
            if (damageable == null) return;
            ProximityCheck(damageable.transform.transform, () => ONKillCallback?.Invoke(damageable),
                () => { }, interactionRange, _lastInteractionDistance,
                () => damageable.IsDead);
        }

        private void ProximityCheck(Transform target, Action onEnter, Action onExit, float range,
            float lastCheckedRange, Func<bool> onEnterConditions)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance >= interactionRange)
            {
                if (distance >= _lastInteractionDistance && _lastInteractionDistance != 0)
                {
                    onExit?.Invoke();
                    _lastInteractionDistance = 0;
                }
            }
            else if (onEnterConditions != null && _lastInteractionDistance == 0 && onEnterConditions.Invoke())
            {
                _lastInteractionDistance = distance;
                onEnter?.Invoke();
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
                            bool result = pickup.OnPickup((Weapon) args);
                            if (result)
                            {
                                ONPickupCallback?.Invoke(pickup);
                            }

                            return !result;
                        }

                        return true;

                    case IInteractable interactable:
                        float distance = Vector3.Distance(transform.position, interactable.transform.position);

                        if (distance >= interactionRange || !CanBeInteracted(interactable)) return true;
                        switch (interactable.InputType)
                        {
                            case InteractionInput.Hold:
                                if (InputListener.GetKey(Interact))
                                {
                                    ONInteractionCallback?.Invoke(interactable);
                                    interactable.OnInteract(_player);
                                }

                                break;
                            case InteractionInput.Press:
                                if (InputListener.GetKeyDown(Interact))
                                {
                                    ONInteractionCallback?.Invoke(interactable);
                                    interactable.OnInteract(_player);
                                }

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

                dotProduct = Vector3.Dot(direction, _player.CameraController.PlayerCamera.transform.forward.normalized);
                return dotProduct >= 0.9f;
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if (!showPickupRange) return;
            Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, detectionRange);

            Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, interactionRange);
        }
    }
}