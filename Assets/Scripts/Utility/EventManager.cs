using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Utility
{
    

    public class EventManager
    {
        public enum Event
        {
            Add,
            Remove
        }
        private static readonly Dictionary<string, Func<object, object>> EventDictionary =
            EventDictionary = new Dictionary<string, Func<object, object>>();

        /// <summary>
        /// Adds a method to a dictionary of methods as listeners. The listener is then called by a set string in the TriggerEvent method.
        /// </summary>
        /// <param name="eventName">The listener's name.</param>
        /// <param name="listener">The method this event manager will listen to.</param>
        public static void AddListener(string eventName, Func<object, object> listener)
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

        /// <summary>
        /// Removes a method from a dictionary based of it's pre-assinged name.
        /// </summary>
        /// <param name="eventName">The listener's name used to remove</param>
        /// <param name="listener">The method itself to remove</param>
        public static void RemoveListener(string eventName, Func<object, object> listener)
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                someEvent.RemoveListener(listener);
            }
        }


        /// <summary>
        /// Triggers a method by using a pre-set name assigned in the AddListener method.
        /// </summary>
        /// <param name="eventName">The name that will be used to find any relevant methods to call.</param>
        /// <param name="args">A single argument to assign to a method. Leave this parameter empty if no arguments exist within the saved method.</param>
        /// <returns>Returns any result gained from any called methods. By default returns null. </returns>
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