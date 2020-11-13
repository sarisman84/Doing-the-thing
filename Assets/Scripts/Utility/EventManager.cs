using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Utility
{
    public class EventManager
    {
        public enum ManagerAction
        {
            Add,
            Remove
        }
        // private static readonly Dictionary<string, Func<object, object>> EventDictionary = new Dictionary<string, Func<object, object>>();
        // private static readonly  Dictionary<string, Func<object, object, object, object, object, object, object>> FuncDictionary = new Dictionary<string, Func<object, object, object, object, object, object, object>>();

        private static readonly Dictionary<string, List<Delegate>> EventDictionary =
            new Dictionary<string, List<Delegate>>();

        /// <summary>
        /// Adds a method to a dictionary of methods as listeners. The listener is then called by a set string in the TriggerEvent method.
        /// </summary>
        /// <param name="eventName">The listener's name.</param>
        /// <param name="listener">The method this event manager will listen to.</param>
        public static void AddListener<TEvent>(string eventName, TEvent listener) where TEvent : Delegate
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                someEvent.Add(listener);
            }
            else
            {
                someEvent = new List<Delegate>();
                someEvent.Add(listener);
            }

            EventDictionary.Add(eventName.ToLower(), someEvent);
        }

        /// <summary>
        /// Removes a method from a dictionary based of it's pre-assinged name.
        /// </summary>
        /// <param name="eventName">The listener's name used to remove</param>
        /// <param name="listener">The method itself to remove</param>
        public static void RemoveListener<TEvent>(string eventName, TEvent listener) where TEvent : Delegate
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                someEvent.Remove(listener);
            }
        }


        /// <summary>
        /// Triggers a method by using a pre-set name assigned in the AddListener method.
        /// </summary>
        /// <param name="eventName">The name that will be used to find any relevant methods to call.</param>
        /// <param name="args">A argument (or arguments) to assign to a method. Leave this parameter empty if no arguments exist within the saved method.</param>
        /// <returns>Returns any result gained from any called methods. By default returns null. </returns>
        public static object TriggerEvent(string eventName, params object[] args)
        {
            if (EventDictionary.TryGetValue(eventName.ToLower(), out var someEvent))
            {
                object result = null; 
                someEvent.ApplyAction(d => result = d.DynamicInvoke(args));
                return result;
            }

            return default;
        }
    }
}