using System;
using System.Collections;
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
            OnLoopEndETeleportToStart,
            OnLoopEndTargetStart,
            OnLoopEndReverse,
            NoLoop
        }


        private Quaternion GetNextLookDirection(bool ignoreYAxis = true)
        {
            Vector3 lookDirection = (waypointList[_currentWaypoint] - waypointList[_previousWaypoint]).normalized;
            if (ignoreYAxis)
            {
                lookDirection.y = 0;
            }

            return Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection), 0.25f);
        }


        private Vector3 GetNextPosition
        {
            get
            {
                var position = waypointList.Count == 0 || waypointList == null
                    ? Vector3.zero
                    : waypointList[_currentWaypoint];
                return position;
            }
        }


        public Vector3 entityCheckSizeOffset = Vector3.zero;
        public Vector3 entityCheckOffset = Vector3.zero;
        public LayerMask detectionMask;
        public List<Vector3> waypointList;
        public bool rotateTowardsDirection, rotateTowardsHorizontalDirection;
        public bool runAtAwake;

        public LoopMode loopMode = LoopMode.OnLoopEndTargetStart;
        public float startupDelay = 0, intermissionDelay = 0, platformSpeed = 2f;

        public bool showDebugMessages;


        private Vector3 TopPosition => transform.position + transform.up * transform.localScale.y;
        private List<Collider> _foundObjects = new List<Collider>();
        [SerializeField] private int _currentWaypoint, _previousWaypoint;
        private Coroutine _platformMovementCoroutine, _entityGrabCoroutine;
        private Vector3 _delta;
        private bool _isMovingFlag;

        private void Awake()
        {
            if (runAtAwake)
                Move(loopMode, startupDelay, intermissionDelay, platformSpeed);
        }

        public void MoveToWaypoint(float platformSpeed, float startupDelay, int waypoint)
        {
            if (_platformMovementCoroutine != null)
                StopCoroutine(_platformMovementCoroutine);

            _platformMovementCoroutine =
                StartCoroutine(MoveToWaypoints(platformSpeed, startupDelay, 0, LoopMode.NoLoop, waypoint));
        }

        public void Move(LoopMode loopMode, float startupDelay, float intermissionDelay, float platformSpeed)
        {
            if (_platformMovementCoroutine != null)
                StopCoroutine(_platformMovementCoroutine);

            _platformMovementCoroutine = StartCoroutine(MoveToWaypoints(platformSpeed, startupDelay, intermissionDelay,
                loopMode, (int[]) waypointList.GetIndexes()));
        }

        public void Stop()
        {
            StopCoroutine(_platformMovementCoroutine);
            StopCoroutine(_entityGrabCoroutine);
        }

        private IEnumerator MoveToWaypoints(float platformSpeed, float onStartupDelay = 0, float onReachDelay = 0,
            LoopMode loopMode = LoopMode.NoLoop, params int[] waypoints)
        {
            if (showDebugMessages)
            {
                Debug.Log(
                    $"Initializing Platform Movement: Waypoint indexes Count vs Actual Count: {waypoints.Length}/{waypointList.Count} - Loop Mode: {loopMode}",
                    gameObject);
                for (int i = 0; i < waypoints.Length; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    Debug.Log($"Registered Index: {waypoints[i]}");
                }
            }


            yield return new WaitForSeconds(onStartupDelay);
            _isMovingFlag = true;
            if (_entityGrabCoroutine != null)
                StopCoroutine(_entityGrabCoroutine);
            _entityGrabCoroutine = StartCoroutine(CarryEntities(platformSpeed));
            bool[] reachedDestinationFlags = new bool[waypoints.Length];
            int flagIndex = 0;

            //Local method to increment the flag index.
            int IncrementFlagIndex(bool[] bools, int i, LoopMode mode)
            {
                if (bools[i])
                {
                    i++;
                    switch (mode)
                    {
                        case LoopMode.OnLoopEndETeleportToStart:
                        case LoopMode.OnLoopEndTargetStart:
                            if (i > bools.Length - 1)
                            {
                                i = 0;
                                if (mode == LoopMode.OnLoopEndETeleportToStart)
                                    transform.position = waypointList[i];
                            }

                            bools[i] = false;
                            break;


                        case LoopMode.OnLoopEndReverse:
                            bool isOutSideOfList = i >= bools.Length - 1 && i > 0;
                            i = isOutSideOfList ? i - 1 : i + 1;
                            bools[i] = false;
                            break;

                        case LoopMode.NoLoop:

                            i = Mathf.Clamp(i, 0, bools.Length - 1);
                            break;
                    }
                }

                return i;
            }


            while (reachedDestinationFlags.Any(d => !d))
            {
                if (waypoints[flagIndex] >= waypointList.Count)
                {
                    Debug.LogWarning($"Waypoint[{waypoints[flagIndex]}] is out of range. Skipping waypoint");
                    reachedDestinationFlags[flagIndex] = loopMode == LoopMode.NoLoop;
                    flagIndex = IncrementFlagIndex(reachedDestinationFlags, flagIndex, loopMode);
                    continue;
                }


                _currentWaypoint = waypoints[flagIndex];
                _foundObjects = FindEntities();

                if (transform.position.IsInTheVicinityOf(waypointList[_currentWaypoint], 0.1f))
                {
                    _previousWaypoint = _currentWaypoint;

                    reachedDestinationFlags[flagIndex] = true;

                    flagIndex = IncrementFlagIndex(reachedDestinationFlags, flagIndex, loopMode);
                    yield return new WaitForSeconds(onReachDelay);
                    continue;
                }

                _delta = waypointList.Count != 0
                    ? (GetNextPosition - transform.position).normalized
                    : Vector3.forward;


                transform.AddPosition(_delta * (platformSpeed * Time.deltaTime));
                if (rotateTowardsDirection || rotateTowardsHorizontalDirection)
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        GetNextLookDirection(rotateTowardsHorizontalDirection), 0.25f);

                yield return new WaitForEndOfFrame();
            }

            _isMovingFlag = false;
            StopCoroutine(_entityGrabCoroutine);
        }

        private IEnumerator CarryEntities(float platformSpeed)
        {
            while (_isMovingFlag)
            {
                if (_foundObjects.Exists(c => c.CompareTag("Player")))
                {
                    PlayerController.Move(_foundObjects.Find(c => c.CompareTag("Player")),
                        _delta * (platformSpeed * Time.fixedDeltaTime));
                }

                _foundObjects.ApplyAction(c =>
                {
                    c.transform.position += _delta * (platformSpeed * Time.fixedDeltaTime);
                    if (!c.CompareTag("Player"))
                        c.transform.rotation = Quaternion.LookRotation((c.transform.right - _delta).normalized);
                });
                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }


        private List<Collider> FindEntities()
        {
            return Physics.OverlapBox(TopPosition + entityCheckOffset,
                    (transform.localScale + entityCheckSizeOffset) / 2f, transform.rotation, detectionMask)
                .Where(c => c.gameObject != gameObject)
                .ToList();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _foundObjects.Exists(c => c != null) ? Color.green : Color.red;
            Gizmos.DrawCube(TopPosition + entityCheckOffset, (transform.localScale + entityCheckSizeOffset));
        }
    }
}