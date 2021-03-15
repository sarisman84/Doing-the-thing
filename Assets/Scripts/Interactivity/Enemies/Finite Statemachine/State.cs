using UnityEngine;

namespace Interactivity.Enemies.Finite_Statemachine
{
    [CreateAssetMenu(fileName = "New State Asset", menuName = "PluggableAI/State", order = 0)]
    public class State : ScriptableObject
    {
        public Action[] actions;
        public Color sceneGizmoColor = Color.gray;
        public void UpdateState(StateController controller)
        {
            DoActions(controller);
        }

        private void DoActions(StateController controller)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].Act(controller);
            }
        }
    }
}