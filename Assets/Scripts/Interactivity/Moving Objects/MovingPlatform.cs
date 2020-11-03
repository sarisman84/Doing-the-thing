using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using Utility;

namespace Interactivity.Moving_Objects
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3 entityCheckSizeOffset = Vector3.zero;
        public Vector3 entityCheckOffset = Vector3.zero;
        private Vector3 TopPosition => transform.position + transform.up * transform.localScale.y;

        public List<Collider> foundObjects;
        [Space] public float platformSpeed = 2;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame

        private Vector3 delta;

        void Update()
        {
            foundObjects = Physics.OverlapBox(TopPosition + entityCheckOffset,
                    (transform.localScale + entityCheckSizeOffset) / 2f, transform.rotation)
                .Where(c => c.gameObject != gameObject)
                .ToList();
            delta = Vector3.forward * platformSpeed;
            transform.position += delta * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (foundObjects.Exists(c => c.CompareTag("Player")))
            {
                EventManager.TriggerEvent(FirstPersonController.MoveEntityEvent, delta * Time.fixedDeltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = foundObjects.Exists(c => c != null) ? Color.green : Color.red;
            Gizmos.DrawCube(TopPosition + entityCheckOffset, (transform.localScale + entityCheckSizeOffset));
        }
    }
}