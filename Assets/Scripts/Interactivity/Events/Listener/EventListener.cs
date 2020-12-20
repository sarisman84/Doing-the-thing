﻿using System;
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
                                instanceEvent.Subscribe<Func<GameObject, bool>>((entity) =>
                                {
                                    bool areEqual = false;
                                    foreach (GameObject targetEntity in eventDefinition.entityComparison)
                                    {
                                        areEqual = targetEntity.GetInstanceID() ==
                                                   entity.GetInstanceID();
                                        if (areEqual)
                                            eventDefinition.InvokeEvent(entity);
                                    }

                                    return areEqual;
                                });
                                break;
                            case EventEdit.Remove:
                                instanceEvent.Unsubcribe<Func<GameObject, bool>>((entity) =>
                                {
                                    bool areEqual = false;
                                    foreach (GameObject targetEntity in eventDefinition.entityComparison)
                                    {
                                        areEqual = targetEntity.GetInstanceID() ==
                                                   entity.GetInstanceID();
                                        if (areEqual)
                                            eventDefinition.InvokeEvent(entity);
                                    }
                                    return areEqual;
                                });
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
        public UnityEvent defaultUnityEvent;
        public UnityEvent<GameObject> entityUnityEvent;


        public void InvokeEvent(object arg = null)
        {
            switch (arg)
            {
                case GameObject gameObject:
                    entityUnityEvent?.Invoke(gameObject);
                    break;

                default:
                    defaultUnityEvent?.Invoke();
                    break;
            }
        }
    }
}