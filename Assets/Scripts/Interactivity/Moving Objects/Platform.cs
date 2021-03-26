using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utility.Attributes;

namespace Interactivity.Moving_Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Platform : MonoBehaviour
    {
        public enum LoopType
        {
            TeleportToStart,
            MoveToStart,
            Reverse,
            Once
        }

        //This should only do the following:
        /*Move in a certain path
         *Have different modes on how to reset.
         *Be able to configure these settings easily
         */
        public bool moveOnAwake;
        [Space] [VisualisePosition(true)] public List<Vector3> waypointList;
        public LoopType loopType;
        public float speed;


        private Rigidbody _physicsBody;
        private Coroutine _platformCoroutine;
        private Vector3 _localPosition;
        private bool _isReversing;

        private void Awake()
        {
            _physicsBody = GetComponent<Rigidbody>();
            _localPosition = transform.position;
            if (moveOnAwake)
                StartMovingToNextWaypoint(0, speed, loopType);
        }


        public void StartMovingToNextWaypoint(int startingPosition, float speed, LoopType loopType)
        {
            if (_platformCoroutine != null)
                StopMoving();
            _platformCoroutine = StartCoroutine(MovePlatform(startingPosition, -1, 0, speed, loopType));
        }

        public void StartMovingToTargetWaypoint(int targetWaypoint, float platformSpeed, float waitTime,
            LoopType movementLoopType)
        {
            if (_platformCoroutine != null)
                StopMoving();
            _platformCoroutine =
                StartCoroutine(MovePlatform(-1, targetWaypoint, waitTime, platformSpeed, movementLoopType));
        }

        public void StopMoving()
        {
            StopCoroutine(_platformCoroutine);
        }

        private IEnumerator MovePlatform(int startingPosition = -1, int endingPosition = -1, float waitTime = 0,
            float assignedSpeed = 2f,
            LoopType assignedLoopType = LoopType.Once)
        {
            int currentPos = (startingPosition != -1) ? startingPosition : 0;
            bool runLoop = true;
            _physicsBody.MovePosition(_localPosition + waypointList[currentPos]);
            currentPos++;

            int startPos = startingPosition == -1 ? 0 : startingPosition;
            int endPos = endingPosition;
            LoopType currLoopType = assignedLoopType;

            while (runLoop)
            {
                if (endingPosition == -1)
                {
                    currentPos = MoveUsingTheWaypointList(assignedSpeed, assignedLoopType, currentPos, ref runLoop);
                }
                else
                {
                    if (GotoDestination(ref startPos, ref endPos, assignedSpeed, ref currLoopType))
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                }


                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        private bool GotoDestination(ref int startingPosition, ref int endingPosition, float assignedSpeed,
            ref LoopType assignedLoopType)
        {
            if (endingPosition >= waypointList.Count || endingPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(endingPosition),
                    $"Assigned destination points outside of the waypoint list.");


            var position = transform.position;
            Vector3 direction = (_localPosition + waypointList[endingPosition]) - position;
            _physicsBody.MovePosition(position + direction.normalized * (assignedSpeed * Time.fixedDeltaTime));

            if (transform.position.IsInTheVicinityOf(_localPosition + waypointList[endingPosition],
                0.05f * assignedSpeed / waypointList.Count))
            {
                switch (assignedLoopType)
                {
                    case LoopType.TeleportToStart:
                        _physicsBody.MovePosition(_localPosition + waypointList[startingPosition]);
                        break;
                    case LoopType.MoveToStart:
                    case LoopType.Reverse:
                        endingPosition = startingPosition;
                        assignedLoopType = LoopType.Once;
                        return true;
                    case LoopType.Once:
                        StopMoving();
                        break;
                }

                return false;
            }

            return false;
        }

        private int MoveUsingTheWaypointList(float speed, LoopType loopType, int currentPos, ref bool runLoop)
        {
            var position = transform.position;
            Vector3 direction = (_localPosition + waypointList[currentPos]) - position;


            _physicsBody.MovePosition(position + direction.normalized * (speed * Time.fixedDeltaTime));
            if (transform.position.IsInTheVicinityOf(_localPosition + waypointList[currentPos],
                0.05f * speed / waypointList.Count))
            {
                if (currentPos == 0)
                {
                    _isReversing = false;
                }

                currentPos = _isReversing ? currentPos - 1 : currentPos + 1;
                if (currentPos >= waypointList.Count)
                {
                    switch (loopType)
                    {
                        case LoopType.TeleportToStart:
                            currentPos = 0;
                            _physicsBody.MovePosition(_localPosition + waypointList[currentPos]);
                            break;
                        case LoopType.MoveToStart:
                            currentPos = 0;
                            break;
                        case LoopType.Reverse:
                            _isReversing = true;
                            currentPos--;
                            break;
                        case LoopType.Once:
                            runLoop = false;
                            break;
                    }
                }
            }

            return currentPos;
        }


        private Vector3 TopPositionOffset => new Vector3(transform.position.x,
            transform.position.y + (transform.localScale.y / 2f), transform.position.z);


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (waypointList != null)
            {
                float lerpAmm = 1 / waypointList.Count;
                for (var i = 0; i < waypointList.Count; i++)
                {
                    int b = i + 1;
                    if (b >= waypointList.Count)
                    {
                        b = 0;
                    }

                    var waypoint = waypointList[i] + _localPosition;
                    var nextWaypoint = waypointList[b] + _localPosition;
                    Gizmos.color = Color.Lerp(Gizmos.color, Color.red, lerpAmm);
                    Gizmos.DrawSphere(waypoint, 0.25f);
                    Gizmos.DrawLine(waypoint, nextWaypoint);
                    lerpAmm += 0.25f;
                }
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                _localPosition = transform.position;
        }
    }
}