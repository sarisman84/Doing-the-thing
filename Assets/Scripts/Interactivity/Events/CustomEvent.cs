using System;
using System.Reflection;
using System.Threading;
using Extensions;
using Interactivity.Components;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using CountdownEvent = Interactivity.Components.CountdownEvent;

namespace Interactivity.Events
{
    public delegate object GameEvent(object[] args);

    [CreateAssetMenu(fileName = "New Event Asset", menuName = "Event/New Event", order = 0)]
    public class CustomEvent : ScriptableObject
    {
        public GameEvent gameEvent;
        public bool hasTriggered;

        protected virtual void OnEnable()
        {
            ResetEvent();
        }

        public virtual void ResetEvent()
        {
            hasTriggered = false;
        }

        public virtual object Raise(bool triggerEventOne, object[] args)
        {
            if (!hasTriggered)
            {
                hasTriggered = triggerEventOne;
                return gameEvent?.Invoke(args);
            }

            return default;
        }
    }


    public static class EventMessageExtension
    {
        public enum EditMode
        {
            Subscribe,
            Unsubscribe
        }

        public static object Raise(this CustomEvent customEvent, bool triggerEventOnce, params object[] args)
        {
            if (customEvent)
            {
                object result;
                if (customEvent is CountdownEvent countdownEvent)
                    result = countdownEvent.Raise(triggerEventOnce, args);

                result = customEvent.Raise(triggerEventOnce, args);
                return result;
            }

            return default;
        }

        public static object Raise(this CountEntityEvent countEntityEvent, GameObject entityCheck = null, params object[] args)
        {
            if (countEntityEvent)
            {
                object result = countEntityEvent.CountEntity(entityCheck, args);
                return result;
            }

            return default;
        }

        public static object Raise(this DefendEvent defendEvent, bool triggerOnce, int healthCheck = 0,
            params object[] args)
        {
            if (defendEvent)
            {
                object result = defendEvent.DefendRaise(triggerOnce, healthCheck, args);
                return result;
            }

            return default;
        }

        public static void EditEvent<Del, C>(this C customEvent, [NotNull] Del unityEvent,
            EditMode editMode)
            where C : CustomEvent
        {
            object EventParse(object[] args)
            {
                if (unityEvent is Delegate @event)
                    return @event.DynamicInvoke(args);
                (unityEvent as UnityEvent)?.Invoke();
                return null;
            }

            if (customEvent)
                if (unityEvent is Delegate || unityEvent is UnityEvent)
                    switch (editMode)
                    {
                        case EditMode.Subscribe:

                            customEvent.gameEvent += EventParse;

                            break;
                        case EditMode.Unsubscribe:
                            customEvent.gameEvent -= EventParse;
                            break;
                    }
        }


        public static T NullcheckMessage<T>(this T value) where T : CustomEvent
        {
            value = value ? value : ScriptableObject.CreateInstance<T>();
            return value;
        }
    }
}