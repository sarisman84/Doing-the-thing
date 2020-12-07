using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components
{
    public class TimedCondition : MonoBehaviour
    {
        public float timeLimit;
        public bool triggerAtAwake;

        private float _startingTime;
        private bool _hasTimeLimitBeingReached;
        private bool _hasEventTriggered;

        public UnityEvent onTimeReachedEvent;
        public UnityEvent onCountingTimeEvent;

        private void Awake()
        {
            if (triggerAtAwake)
                BeginCount();
        }

        private void BeginCount()
        {
            _startingTime = Time.time;
            _hasEventTriggered = false;
        }

        private void Update()
        {
            _hasTimeLimitBeingReached = Time.time - _startingTime >= timeLimit;
            if (!_hasEventTriggered && _hasTimeLimitBeingReached)
            {
                onTimeReachedEvent?.Invoke();
                _hasEventTriggered = true;
            }else if (!_hasTimeLimitBeingReached)
            {
                onCountingTimeEvent?.Invoke();
            }
        }
    }
}