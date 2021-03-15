using UnityEngine;

namespace Interactivity.Enemies.Finite_Statemachine.Actions
{
    [CreateAssetMenu(fileName = "FILENAME", menuName = "PluggableAI/Actions/Patrol", order = 0)]
    public class PatrolAction : Action
    {
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }

        private void Patrol(StateController controller)
        {
            controller.navMeshAgent.destination = controller.waypointList[controller.nextWaypoint].position;
            controller.navMeshAgent.Resume();

            if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance &&
                !controller.navMeshAgent.pathPending) ;
            {
                controller.nextWaypoint = (controller.nextWaypoint + 1) % controller.waypointList.Count;
            }
        }
    }
}