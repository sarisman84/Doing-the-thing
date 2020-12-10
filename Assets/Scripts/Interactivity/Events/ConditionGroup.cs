using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Events
{
    public class ConditionGroup : MonoBehaviour
    {
        public List<CustomEvent> customEvents = new List<CustomEvent>();
        public bool triggerResponseOnce;
        public UnityEvent response;
        public CustomEvent customResponse;

        private bool _hasTriggered;

        private void Update()
        {
            if (customEvents.All(c =>
            {
                return c.hasTriggered;
            }) && !_hasTriggered)
            {
                Debug.Log("Conditions met");
                _hasTriggered = triggerResponseOnce;
                response?.Invoke();
                customResponse.ResetEvent();
                customResponse.Raise(true, 420, "Blaze it");
            }
        }
    }
}