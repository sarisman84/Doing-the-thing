using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Interactivity.Moving_Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Platform : MonoBehaviour
    {
        //This should only do the following:
        /*Move in a certain path
         *Have different modes on how to reset.
         *Be able to configure these settings easily
         */
        public bool moveOnAwake;
        public List<Vector3> waypointList;
        public int maxPhysicalEntitiesOnPlatform = 50;
        public float speed;

        private Rigidbody _physicsBody;
        private Coroutine _platformCoroutine;
        private Vector3 _localPosition;

        private void Awake()
        {
            _physicsBody = GetComponent<Rigidbody>();
            _localPosition = transform.position;
            if (moveOnAwake)
                StartMoving();
        }


        public void StartMoving()
        {
            _platformCoroutine = StartCoroutine(MovePlatform(0));
        }

        public void StopMoving()
        {
            StopCoroutine(_platformCoroutine);
        }

        private IEnumerator MovePlatform(int startingPosition)
        {
            int currentPos = startingPosition;
            while (true)
            {
                Vector3 direction = (_localPosition + waypointList[currentPos]) - transform.position;


                _physicsBody.MovePosition(transform.position + direction.normalized * (speed * Time.fixedDeltaTime));
                if (transform.position.IsInTheVicinityOf(_localPosition + waypointList[currentPos],0.01f))
                {
                    currentPos++;
                    if (currentPos >= waypointList.Count)
                    {
                        currentPos = 0;
                    }
                }

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }


        private Vector3 TopPositionOffset => new Vector3(transform.position.x,
            transform.position.y + (transform.localScale.y / 2f), transform.position.z);


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            float lerpAmm = 0;
            for (var i = 0; i < waypointList.Count; i++)
            {
                int b = i + 1;
                if (b >= waypointList.Count)
                {
                    b = 0;
                }

                var waypoint = waypointList[i];
                var nextWaypoint = waypointList[b];
                Gizmos.color = Color.Lerp(Gizmos.color, Color.red, lerpAmm);
                Gizmos.DrawSphere(_localPosition + waypoint, 0.25f);
                Gizmos.DrawLine(waypoint, nextWaypoint);
                lerpAmm += 0.25f;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                _localPosition = transform.position;
        }
    }
}