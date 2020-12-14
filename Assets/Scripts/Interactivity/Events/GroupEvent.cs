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
        public bool callEventWhileConditionsAreFalse;
        [EnableIf("callEventWhileConditionsAreFalse")]
        public CustomEvent whileConditionsAreFalse;
#if UNITY_EDITOR
        public bool showDebugLogs;
#endif

        public override void OnInvokeEvent()
        {
#if UNITY_EDITOR
            if (showDebugLogs)
                targetEvents.ApplyAction(e =>
                    Debug.Log($"Event ({e.name}) is {(e.IsBeingCalled ? "being called!" : "not being called!")}"));
#endif
            switch (conditionType)
            {
                case ConditionType.All:
                    if (targetEvents.All(e => invertCondition ? !e.IsBeingCalled : e.IsBeingCalled))
                    {
#if UNITY_EDITOR
                        if (showDebugLogs)
                            Debug.Log("All known events are being called!");
#endif
                        base.OnInvokeEvent();
                    }
                    else if (callEventWhileConditionsAreFalse && whileConditionsAreFalse)
                    {
                        IsBeingCalled = false;
                        whileConditionsAreFalse.OnInvokeEvent();
                    }

                    break;
                case ConditionType.Any:
                    if (targetEvents.Any(e => invertCondition ? !e.IsBeingCalled : e.IsBeingCalled))
                    {
#if UNITY_EDITOR
                        if (showDebugLogs)
                            Debug.Log("Some of the known events are being called!");
#endif

                        base.OnInvokeEvent();
                    }
                    else if (callEventWhileConditionsAreFalse && whileConditionsAreFalse)
                    {
                        IsBeingCalled = false;
                        whileConditionsAreFalse.OnInvokeEvent();
                    }

                    break;
            }

            IsBeingCalled = false;
        }
    }
}