using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
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

        private Quaternion GetNextLookDirection =>
            rotateTowardsDirection
                ? Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(
                    (waypointList[_currentWaypoint] -
                     waypointList[_previousWaypoint])), 0.25f)
                : Quaternion.identity;

        private float GetNextDistance => Vector3.Distance(waypointList[_currentWaypoint], transform.position);

        private Vector3 GetNextPosition
        {
            get
            {
                var position = transform.position;
                delta = waypointList.Count != 0
                    ? (waypointList[_currentWaypoint] - position).normalized * platformSpeed
                    : Vector3.forward * platformSpeed;
                return position;
            }
        }

        public Vector3 entityCheckSizeOffset = Vector3.zero;
        public Vector3 entityCheckOffset = Vector3.zero;
        public LayerMask detectionMask;
        private Vector3 TopPosition => transform.position + transform.up * transform.localScale.y;

        public List<Collider> foundObjects;
        [Space] public float platformSpeed = 2;

        public List<Vector3> waypointList;
        public LoopMode loopMode = LoopMode.NormalReset;
        public bool rotateTowardsDirection = true;
        public bool activateOnAwake;
        public bool hasToCompleteItsPath;
        public bool IsActive { get; set; } = true;

        private int _currentWaypoint, _previousWaypoint;
        private bool _reversing;

        void Awake()
        {
            IsActive = activateOnAwake;
            _currentWaypoint = 0;
        }

        // Update is called once per frame

        private Vector3 delta;

        private bool _hasToCompleteItsPath;

        void Update()
        {
            _hasToCompleteItsPath = !transform.position.IsInTheVicinityOf(waypointList[0], 0.1f) &&
                                   !transform.position.IsInTheVicinityOf(waypointList[waypointList.Count - 1], 0.1f) &&
                                   hasToCompleteItsPath;
            if (IsActive || _hasToCompleteItsPath)
                MovePlatform();
        }

        private void MovePlatform()
        {
            foundObjects = FindEntities();
            var position = GetNextPosition;

            if (loopMode == LoopMode.ReverseReset && _currentWaypoint == waypointList.Count - 1)
            {
                _reversing = true;
            }
            else if (_currentWaypoint == 0)
            {
                _reversing = false;
            }

            TrySetNextPosition();

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

                    break;
            }

            if (rotateTowardsDirection)
            {
                transform.rotation = GetNextLookDirection;
            }

            transform.position = position;
        }

        private void TrySetNextPosition(float minDistance = 0.1f)
        {
            if (transform.position.IsInTheVicinityOf(waypointList[_currentWaypoint], minDistance))
            {
                _previousWaypoint = _currentWaypoint;
                _currentWaypoint = _currentWaypoint + 1 >= waypointList.Count
                    ? _reversing ? _currentWaypoint - 1 : 0
                    : _reversing
                        ? _currentWaypoint - 1
                        : _currentWaypoint + 1;
            }
        }

        private List<Collider> FindEntities()
        {
            return Physics.OverlapBox(TopPosition + entityCheckOffset,
                    (transform.localScale + entityCheckSizeOffset) / 2f, transform.rotation, detectionMask)
                .Where(c => c.gameObject != gameObject)
                .ToList();
        }

        private void FixedUpdate()
        {
            // if (foundObjects.Exists(c => c.CompareTag("Player")))
            // {
            //     EventManager.TriggerEvent(PlayerController.MoveEntityEvent, delta * Time.fixedDeltaTime);
            // }
            if (IsActive || _hasToCompleteItsPath)
                foundObjects.ApplyAction(c =>
                {
                    c.transform.position += delta * Time.fixedDeltaTime;
                    if (!c.CompareTag("Player"))
                        c.transform.rotation = Quaternion.LookRotation((c.transform.right - delta).normalized);
                });
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = foundObjects.Exists(c => c != null) ? Color.green : Color.red;
            Gizmos.DrawCube(TopPosition + entityCheckOffset, (transform.localScale + entityCheckSizeOffset));
        }
    }
}