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
        public List<CustomEvent> targetEvents = new List<CustomEvent>();


        protected override void OnEnable()
        {
            base.OnEnable();
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

        public override void OnInvokeEvent()
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

        private bool AreConditionsMet(ConditionType type)
        {
            //Create local array to store current result states
            bool[] individualResults = new bool[targetEvents.Count];
            int resultIndex = 0;

            foreach (CustomEvent eEvent in targetEvents)
            {
                switch (eEvent)
                {
                    case InstanceEvent instanceEvent:
                        individualResults[resultIndex] = instanceEvent.AreAllInstancesBeingCalled();
                        Debug.Log($"Group check ({instanceEvent.name}); {individualResults[resultIndex]}");
                        break;
                    default:
                        individualResults[resultIndex] = eEvent.IsBeingCalled;
                        break;
                }

                //Increment index by 1 and make sure it stays within bounds.
                resultIndex++;
                resultIndex = Mathf.Clamp(resultIndex, 0, targetEvents.Count);
            }

            return type == ConditionType.All ? individualResults.All(r => r) : individualResults.Any(r => r);
        }
    }
}