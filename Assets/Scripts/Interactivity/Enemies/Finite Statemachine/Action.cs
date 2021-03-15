using UnityEngine;

namespace Interactivity.Enemies.Finite_Statemachine
{
  
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);
    }
}