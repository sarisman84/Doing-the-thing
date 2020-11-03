using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Utility
{
    public class EventManager
    {
        private static readonly Dictionary<string, Func<object,object>> EventDictionary =
            EventDictionary = new Dictionary<string, Func<object, object>>();

        public static void ListenTo(string eventName, Func<object,object> listener)
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                someEvent += listener;
            }
            else
            {
                someEvent = listener;
            }

            EventDictionary.Add(eventName.ToLower(), someEvent);
        }

        public static void StopListeningTo(string eventName, Func<object,object> listener)
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                someEvent.RemoveListener(listener);
            }
        }

        public static object TriggerEvent(string eventName, object args = null)
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                return someEvent.Invoke(args);
            }

            return null;
        }
    }
}