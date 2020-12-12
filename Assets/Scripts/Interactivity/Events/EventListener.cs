using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public class EventListener : MonoBehaviour
    {
        public List<EventInfo> events = new List<EventInfo>();

        private void OnEnable()
        {
            events.ApplyAction(e => e.AddResponses());
        }

        private void OnDisable()
        {
            events.ApplyAction(e => e.RemoveResponses());
        }

        private void OnDestroy()
        {
            events.ApplyAction(e => e.RemoveResponses());
        }
    }

    [Serializable]
    public class EventInfo
    {
        public CustomEvent listener;

        [Space] public UnityEvent unityResponse;
        public CustomEvent customResponse;

        public void AddResponses()
        {
            if (listener is CountingEvent countingEvent)
            {
                countingEvent.Subscribe<Action<Collider>>((col) => unityResponse.Invoke());
            }
            else
                listener.Subscribe<Action>(unityResponse.Invoke);

            if (customResponse)
                listener.Subscribe<Action>(() => customResponse.OnInvokeEvent());
        }

        public void RemoveResponses()
        {
            if (listener is CountingEvent countingEvent)
            {
                countingEvent.Subscribe<Action<Collider>>((col) => unityResponse.Invoke());
            }
            else
            listener.Unsubcribe<Action>(unityResponse.Invoke);
            if (customResponse)
                listener.Unsubcribe<Action>(() => customResponse.OnInvokeEvent());
        }
    }
}