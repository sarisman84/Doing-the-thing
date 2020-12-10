using System;
using System.Collections;
using Extensions;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components
{
    [CreateAssetMenu(fileName = "New Time Condition", menuName = "Event/Time Condition", order = 0)]
    public class CountdownEvent : CustomEvent
    {
        private float _startingTime;
        public int targetTime;

        protected override void OnEnable()
        {
            _startingTime = 0;
            hasTriggered = false;
        }

        public override object Raise(bool triggerEventOnce, params object[] args)
        {
            if (_startingTime == 0) ResetCount();
            if (Mathf.Clamp(Time.time - _startingTime, 0, targetTime) == targetTime && !hasTriggered)
            {
                hasTriggered = triggerEventOnce;
                return gameEvent?.Invoke(args);
            }

            return default;
        }


        public void ResetCount()
        {
            _startingTime = Time.time;
        }
    }
}