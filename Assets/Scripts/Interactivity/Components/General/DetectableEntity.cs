using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components.General
{
    public class DetectableEntity : MonoBehaviour
    {
        public UnityEvent<Collider> onDetectionEvent;

        public void OnDetection(Collider col)
        {
            onDetectionEvent?.Invoke(col);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position + Vector3.up * transform.localScale.y, 0.25f * transform.localScale.y);
        }
    }
}