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
        


        private void OnEnable()
        {
            targetEvents.ApplyAction(e => e.Subscribe<Action>(OnInvokeEvent));
        }

        private void OnDisable()
        {
            targetEvents.ApplyAction(e => e.Unsubcribe<Action>(OnInvokeEvent));
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
            foreach (CustomEvent eEvent in targetEvents)
            {
                switch (eEvent)
                {
                    case InstanceEvent instanceEvent:
                        switch (type)
                        {
                            case ConditionType.All:
                                if (!instanceEvent.IsInstanceBeingCalled)
                                    return false;
                                break;
                            case ConditionType.Any:
                                if (instanceEvent.IsInstanceBeingCalled)
                                    return true;
                                break;
                        }

                        break;
                    default:
                        switch (type)
                        {
                            case ConditionType.All:
                                if (!eEvent.IsBeingCalled)
                                    return false;
                                break;
                            case ConditionType.Any:
                                if (eEvent.IsBeingCalled)
                                    return true;
                                break;
                        }

                        break;
                }
            }

            return true;
        }
    }
}