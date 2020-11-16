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
        public enum LoopMode
        {
            ResetTeleport,
            NormalReset,
            ReverseReset
        }


        public Vector3 entityCheckSizeOffset = Vector3.zero;
        public Vector3 entityCheckOffset = Vector3.zero;
        private Vector3 TopPosition => transform.position + transform.up * transform.localScale.y;

        public List<Collider> foundObjects;
        [Space] public float platformSpeed = 2;

        public List<Vector3> waypointList;
        public LoopMode loopMode = LoopMode.NormalReset;
        public bool rotateTowardsDirection = true;

        private int _currentWaypoint, _previousWaypoint;
        private float _distanceToCurrentWaypoint;
        private bool _reversing;

        // Start is called before the first frame update
        void Awake()
        {
            _currentWaypoint = 0;
        }

        // Update is called once per frame

        private Vector3 delta;

        void Update()
        {
            foundObjects = Physics.OverlapBox(TopPosition + entityCheckOffset,
                    (transform.localScale + entityCheckSizeOffset) / 2f, transform.rotation)
                .Where(c => c.gameObject != gameObject)
                .ToList();


            var position = transform.position;
            delta = waypointList.Count != 0
                ? (waypointList[_currentWaypoint] - position).normalized * platformSpeed
                : Vector3.forward * platformSpeed;


            _distanceToCurrentWaypoint = Vector3.Distance(waypointList[_currentWaypoint], transform.position);
            if (loopMode == LoopMode.ReverseReset && _currentWaypoint == waypointList.Count - 1)
            {
                _reversing = true;
            }
            else if (_currentWaypoint == 0)
            {
                _reversing = false;
            }

            if (_distanceToCurrentWaypoint <= 0.1f)
            {
                _previousWaypoint = _currentWaypoint;
                _currentWaypoint = _currentWaypoint + 1 >= waypointList.Count
                    ? _reversing ? _currentWaypoint - 1 : 0
                    : _reversing
                        ? _currentWaypoint - 1
                        : _currentWaypoint + 1;
            }

            Quaternion lookDirection = Quaternion.identity;
            switch (loopMode)
            {
                case LoopMode.ResetTeleport:
                case LoopMode.NormalReset:
                case LoopMode.ReverseReset:
                    if (_currentWaypoint == 0 && loopMode == LoopMode.ResetTeleport)
                    {
                        position = waypointList[_currentWaypoint];
                    }
                    else
                    {
                        position += delta * Time.deltaTime;
                    }

                    lookDirection = rotateTowardsDirection
                        ? Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(
                            (waypointList[_currentWaypoint] -
                             waypointList[_previousWaypoint])), 0.25f)
                        : Quaternion.identity;
                    break;
            }

            if (rotateTowardsDirection)
            {
                transform.rotation = lookDirection;
            }

            transform.position = position;
        }

        private void FixedUpdate()
        {
            if (foundObjects.Exists(c => c.CompareTag("Player")))
            {
                EventManager.TriggerEvent(PlayerController.MoveEntityEvent, delta * Time.fixedDeltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = foundObjects.Exists(c => c != null) ? Color.green : Color.red;
            Gizmos.DrawCube(TopPosition + entityCheckOffset, (transform.localScale + entityCheckSizeOffset));
        }
    }
}