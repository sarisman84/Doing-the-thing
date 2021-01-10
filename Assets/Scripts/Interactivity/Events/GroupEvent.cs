using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using Utility.Attributes;

namespace Interactivity.Events
{
    [CreateAssetMenu(fileName = "New Group Event", menuName = "Event/Group Event", order = 0)]
    public class GroupEvent : CustomEvent
    {
        public enum ConditionType
        {
            All,
            Any
        }

        public bool invertCondition;
        public ConditionType conditionType;
        [Expose]
        public List<CustomEvent> targetEvents = new List<CustomEvent>();

#if UNITY_EDITOR
        public bool showDebugMessages;
        [SerializeField] private bool triggerOnce;
#endif

        protected void OnEnable()
        {
            targetEvents.ApplyAction(SubscribeMethod);
        }

        private void SubscribeMethod(CustomEvent customEvent)
        {
            switch (customEvent)
            {
                case InstanceEvent instanceEvent:
                    instanceEvent.Subscribe<Action<GameObject>>(id =>
                        InstanceEvent.InstanceCheck<Action>(id, instanceEvent, OnInvokeEvent));
                    break;
                default:
                    customEvent.Subscribe<Action>(OnInvokeEvent);
                    break;
            }
        }


        private void OnDisable()
        {
            targetEvents.ApplyAction(UnsubscribeMethod);
        }

        private void UnsubscribeMethod(CustomEvent customEvent)
        {
            switch (customEvent)
            {
                case InstanceEvent instanceEvent:
                    instanceEvent.Unsubcribe<Action<GameObject>>(id =>
                        InstanceEvent.InstanceCheck<Action>(id, instanceEvent, OnInvokeEvent));
                    break;
                default:
                    customEvent.Unsubcribe<Action>(OnInvokeEvent);
                    break;
            }
        }

        public void OnInvokeEvent()
        {
            if (invertCondition && !AreConditionsMet(conditionType))
            {
                base.OnInvokeEvent();
            }
            else if (AreConditionsMet(conditionType))
            {
                base.OnInvokeEvent();
            }

            if (!triggerOnce)
                IsBeingCalled = false;
        }

        public bool IsBeingCalled { get; set; }

        private bool AreConditionsMet(ConditionType type)
        {
            //Create local array to store current result states
            bool[] individualResults = new bool[targetEvents.Count];
            int resultIndex = 0;

            foreach (CustomEvent eEvent in targetEvents)
            {
                individualResults[resultIndex] = false;
                switch (eEvent)
                {
                    case InstanceEvent instanceEvent:
                        individualResults[resultIndex] = instanceEvent.AreAllInstancesBeingCalled();
#if UNITY_EDITOR
                        if (showDebugMessages)
                        {
                            int index = 0;
                            instanceEvent.CurrentStatus.ApplyAction(b =>
                            {
                                Debug.Log(
                                    $"Current state of {instanceEvent.name}'s child state [{index}] is {(b ? "being called" : "not being called")}");
                                index++;
                            });
                        }

#endif
                        break;
                    default:
                        individualResults[resultIndex] = eEvent.eventBehaivour.IsMet();
                        break;
                }

                //Increment index by 1 and make sure it stays within bounds.
                resultIndex++;
                resultIndex = Mathf.Clamp(resultIndex, 0, individualResults.Length - 1);
            }

            return type == ConditionType.All ? individualResults.All(r => r) : individualResults.Any(r => r);
        }
    }
}