using System.Collections.Generic;
using Extensions;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components
{
    public enum ZoneType
    {
        Sphere,
        Box
    }

    public class DetectionArea : MonoBehaviour
    {
        public UnityEvent onZoneEnter;
        public UnityEvent onZoneExit;
        public UnityEvent onZoneStay;


        public ZoneType zoneType;

        public LayerMask detectionMask;

        [HideInInspector] public new Collider collider;

        bool IsColliderLayerInDetectionMask(Collider other)
        {
            bool result = detectionMask.ToInteger().Contains(other.gameObject.layer);
            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsColliderLayerInDetectionMask(other))
            {
                onZoneEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsColliderLayerInDetectionMask(other))
            {
                onZoneExit.Invoke();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsColliderLayerInDetectionMask(other))
            {
                onZoneStay.Invoke();
            }
        }


#if UNITY_EDITOR
        public Color zoneColor;

        public enum ZoneTriggerType
        {
            OnEnter,
            OnExit,
            OnStay,
            OnEnterOrStay,
            OnStayOrExit,
            OnExitOrEnter,
            All
        }

        public ZoneTriggerType triggerType;

        private void OnValidate()
        {
            switch (zoneType)
            {
                case ZoneType.Sphere:
                    if (collider is BoxCollider)
                        collider.enabled = false;


                    collider = !GetComponent<SphereCollider>().Equals(null)
                        ? GetComponent<SphereCollider>()
                        : gameObject.AddComponent<SphereCollider>();


                    break;
                case ZoneType.Box:
                    if (collider is SphereCollider)
                        collider.enabled = false;

                    collider = !GetComponent<BoxCollider>().Equals(null)
                        ? GetComponent<BoxCollider>()
                        : gameObject.AddComponent<BoxCollider>();

                    break;
            }

            if (collider)
            {
                collider.isTrigger = true;
                collider.enabled = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (collider.Equals(null)) return;
            Gizmos.color = zoneColor;
            switch (zoneType)
            {
                case ZoneType.Sphere:
                    if (collider is SphereCollider sphereCollider)
                        Gizmos.DrawSphere(transform.position, sphereCollider.radius);
                    break;
                case ZoneType.Box:
                    if (collider is BoxCollider)
                        Gizmos.DrawCube(transform.position, Vector3.Max(collider.bounds.size, transform.localScale));
                    break;
            }
        }
#endif
    }
}