using UnityEngine;

namespace Interactivity.Events
{
    [CreateAssetMenu(fileName = "New Time Condition", menuName = "Event/Timed Event", order = 0)]
    public class TimeEvent : CustomEvent
    {
        private float _startingTime;
        public int targetTime;

        public override void OnInvokeEvent()
        {
            if (Time.time - _startingTime >= targetTime)
            {
                base.OnInvokeEvent();
            }

            IsBeingCalled = false;
        }

        public void BeginCount()
        {
            _startingTime = Time.time;
        }
    }
}