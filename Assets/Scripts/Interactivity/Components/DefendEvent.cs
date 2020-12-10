using System;
using Extensions;
using Interactivity.Events;
using UnityEngine;

namespace Interactivity.Components
{
    [CreateAssetMenu(fileName = "New Defence Event", menuName = "Event/Defence", order = 0)]
    public class DefendEvent : CustomEvent
    {
        public bool triggerWhileAlive;
        public object DefendRaise(bool triggerEventOnce, int healthAmount, params object[] args)
        {
            if (healthAmount <= 0 && !hasTriggered)
            {
                hasTriggered = triggerEventOnce;
                return gameEvent?.Invoke(args);
            }
            if (healthAmount > 0 && triggerWhileAlive)
            {
                return gameEvent?.Invoke(args);
            }

            return default;
        }
    }
}