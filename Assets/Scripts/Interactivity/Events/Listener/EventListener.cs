using System;
using System.Collections.Generic;
using Player;
using Player.Weapons;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Events.Listener
{
    public class EventListener : MonoBehaviour
    {
        public EventDefinition[] events;
        public EventEdit eventEditType;

        public enum EventEdit
        {
            Listen,
            Remove
        }

        private void OnEnable()
        {
            EditEventsArray(EventEdit.Listen);
        }

        private void OnDisable()
        {
            EditEventsArray(EventEdit.Remove);
        }

        private void EditEventsArray(EventEdit type)
        {
            foreach (EventDefinition eventDefinition in events)
            {
                switch (eventDefinition.eventListener)
                {
                    case InstanceEvent instanceEvent:
                        switch (type)
                        {
                            case EventEdit.Listen:
                                instanceEvent.Subscribe<Action<GameObject>>(id =>
                                    InstanceEvent.InstanceCheck(id, eventDefinition, instanceEvent));
                                break;
                            case EventEdit.Remove:
                                instanceEvent.Unsubcribe<Action<GameObject>>(id =>
                                    InstanceEvent.InstanceCheck(id, eventDefinition, instanceEvent));
                                break;
                        }

                        break;
                    default:
                        switch (type)
                        {
                            case EventEdit.Listen:
                                eventDefinition.eventListener.Subscribe<Action>(() => eventDefinition.InvokeEvent());
                                break;
                            case EventEdit.Remove:
                                eventDefinition.eventListener.Unsubcribe<Action>(() => eventDefinition.InvokeEvent());
                                break;
                        }

                        break;
                }
                
            }
        }
    }

    [Serializable]
    public class EventDefinition
    {
        public CustomEvent eventListener;
        public GameObject[] entityComparison;
        public bool useCustomArg = false;
        
        public UltEvent defaultUnityEvent;
        public UltEvent instanceUnityEvent;

        public void InvokeEvent(object arg = null)
        {
            switch (arg)
            {
                case GameObject gameObject:
                    instanceUnityEvent?.Invoke();
                    break;
                default:
                    defaultUnityEvent?.Invoke();
                    break;
            }
        }
    }
}