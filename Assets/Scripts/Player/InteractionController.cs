using System;
using System.Collections;
using System.Collections.Generic;
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
using static Player.InputController.KeyCode;

namespace Player
{
    public class InteractionController : MonoBehaviour
    {
        public bool useRaycast;
        [EnableIf("useRaycast")] public float interactionRange;
        [EnableIf("useRaycast")] public LayerMask interactionMask;
        public bool useOverlapSphere;
        [EnableIf("useOverlapSphere")] public float overlapSphereRange;
        [EnableIf("useOverlapSphere")] public LayerMask detectionMask;
        public bool showDebug;

        public event Action<RaycastHit> ONInteractionEvent;
        public event Action<Collider> ONDetectionEvent;
        private Camera _cam;
        private Collider[] _cachedFoundColliders;
        private Coroutine _coroutinedUpdateLoop;


        private static CustomEvent accessInteractionComponent;


        public static InteractionController GetInteractionController(GameObject owner)
        {
            if (accessInteractionComponent)
                return (InteractionController) accessInteractionComponent.OnInvokeEvent(owner);
            return null;
        }

        private void Awake()
        {
            accessInteractionComponent =
                CustomEvent.CreateEvent<Func<InteractionController>>(GetInteractionComponent, gameObject);
            _cam = Camera.main;
        }

        private void OnEnable()
        {
            accessInteractionComponent = accessInteractionComponent
                ? accessInteractionComponent
                : CustomEvent.CreateEvent<Func<InteractionController>>(GetInteractionComponent,
                    gameObject);
            ONDetectionEvent += InteractWithDetectableEntities;
            _coroutinedUpdateLoop = StartCoroutine(CoroutineUpdateLoop());
        }

        private IEnumerator CoroutineUpdateLoop()
        {
            while (true)
            {
                if (useOverlapSphere)
                    yield return OverlapSphereDetection(transform.position, overlapSphereRange, detectionMask);
                else
                    yield return new WaitForEndOfFrame();
            }

            yield return null;
        }


        private void OnDisable()
        {
            accessInteractionComponent.RemoveEvent<Func<InteractionController>>(GetInteractionComponent);
            ONDetectionEvent -= InteractWithDetectableEntities;

            if (_coroutinedUpdateLoop != null)
                StopCoroutine(_coroutinedUpdateLoop);
        }

        private InteractionController GetInteractionComponent()
        {
            return this;
        }


        private void InteractWithDetectableEntities(Collider obj)
        {
            DetectableEntity entity = obj.GetComponent<DetectableEntity>();
            if (entity)
            {
                entity.OnDetect(GetComponent<Collider>(), overlapSphereRange);
            }
        }


        private void Update()
        {
            if (useRaycast)
                RaycastDetection(_cam.transform.position, _cam.transform.forward, interactionRange, interactionMask);
        }

        private void LateUpdate()
        {
            // cachedFoundColliders = new Collider[300];
        }

        private void RaycastDetection(Vector3 origin, Vector3 direction, float mask,
            LayerMask interactionMask)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit hitInfo;
            Color rayColor = Color.red;

            if (Physics.Raycast(ray, out hitInfo, mask, interactionMask))
            {
                rayColor = Color.green;
                ONInteractionEvent?.Invoke(hitInfo);
            }

            if (showDebug)
                Debug.DrawRay(ray.origin, ray.direction * mask, rayColor);
        }

        private IEnumerator OverlapSphereDetection(Vector3 origin, float radius, LayerMask mask)
        {
            Collider[] foundColliders = new Collider[50];
            Physics.OverlapSphereNonAlloc(origin, radius, foundColliders, mask);

            if (foundColliders.Length == 0) yield break;

            foreach (var foundObject in foundColliders)
            {
                if (foundObject)
                    ONDetectionEvent?.Invoke(foundObject);
            }

            _cachedFoundColliders = foundColliders;
            yield return new WaitForSeconds(0.1f);
        }


        private void OnDrawGizmos()
        {
            if (showDebug)
            {
                if (useOverlapSphere)
                {
                    Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
                    Gizmos.DrawSphere(transform.position, overlapSphereRange);
                    Gizmos.color -= new Color(0.4f, 0.4f, 0.4f, 0);
                    Gizmos.DrawWireSphere(transform.position, overlapSphereRange);

                    if (_cachedFoundColliders == null || _cachedFoundColliders.Length != 0)
                    {
                        foreach (var collider in _cachedFoundColliders)
                        {
                            if (!collider) continue;
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere(collider.transform.position, 1f);
                        }
                    }
                }

                if (useRaycast && Application.isEditor && !Application.isPlaying)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactionRange);
                }
            }
        }
    }
}