using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Interactivity.Enemies.Finite_Statemachine
{
    public class StateController : MonoBehaviour
    {
        public State currentState;

        [HideInInspector] public int nextWaypoint;

        [HideInInspector] public List<Transform> waypointList;
        [HideInInspector] public NavMeshAgent navMeshAgent;

        private void Update()
        {
            currentState.UpdateState(this);
        }

        private void OnDrawGizmos()
        {
            if (!currentState)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position, 2f);
            }
        }
    }
}