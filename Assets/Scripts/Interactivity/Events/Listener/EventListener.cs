using System;
using System.Collections.Generic;
using Extensions;
using UltEvents;
using UnityEngine;
using Utility.Attributes;

namespace Interactivity.Events.Listener
{
    public class EventListener : MonoBehaviour
    {
        public List<Event> events;

        private void OnEnable()
        {
            UpdateRegistrationOnEvents(true);
        }

        private void OnDisable()
        {
            UpdateRegistrationOnEvents(false);
        }


        void UpdateRegistrationOnEvents(bool register)
        {
            events.ApplyAction(e =>
            {
                e.conditions.ApplyAction(c =>
                {
                    if (e.eventToListenTo == null) return;
                    if (register)
                    {
                        try
                        {
                            e.eventToListenTo.Subscribe<Action>(e.responseToEvent.Invoke, c);
                        }
                        catch (Exception exception)
                        {
                            Debug.Log(
                                $"{exception} -> Is event null? {(e.eventToListenTo == null ? "Yep." : "Nope.")}| Is condition null? {(c == null ? "Yep." : "Nope.")} | Is the response null? {(e.responseToEvent == null ? "Yep." : "Nope.")}");
                        }
                    }
                    else
                    {
                        try
                        {
                            e.eventToListenTo.Unsubcribe<Action>(e.responseToEvent.Invoke, c);
                        }
                        catch (Exception exception)
                        {
                            Debug.Log(
                                $"{exception} -> Is event null? {(e.eventToListenTo == null ? "Yep." : "Nope.")}| Is condition null? {(c == null ? "Yep." : "Nope.")} | Is the response null? {(e.responseToEvent == null ? "Yep." : "Nope.")}");
                        }
                    }
                });
            });
        }
    }

    [Serializable]
    public struct Event
    {
        [Expose] public CustomEvent eventToListenTo;
        public List<GameObject> conditions;
        public UltEvent responseToEvent;
    }
}