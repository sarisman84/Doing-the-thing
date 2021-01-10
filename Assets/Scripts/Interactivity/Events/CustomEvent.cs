using System;
using System.Reflection;
using System.Threading;
using Extensions;
using Interactivity.Components;
using Interactivity.Events.Conditions;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Utility.Attributes;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public delegate void EmptyEvent();


    public delegate object ObjectEvent<in T>(params T[] args);

    [CreateAssetMenu(fileName = "New Event Asset", menuName = "Event/New Event", order = 0)]
    public class CustomEvent : ScriptableObject
    {
        [Expose] public CustomEventBehaivour eventBehaivour;
        protected event ObjectEvent<object> GameEvent;


        public virtual void OnInvokeEvent(GameObject gameObject = null)
        {
            if (eventBehaivour != null && eventBehaivour.IsMet(gameObject))
                GameEvent?.Invoke(gameObject);
            else
            {
                GameEvent?.Invoke();
            }
        }


        public virtual void Subscribe<TDel>(TDel method, params object[] args) where TDel : Delegate
        {
            //args[]
            if (eventBehaivour != null)
            {
                GameEvent += (localArgs) =>
                {
                    object[] newArgs = new object[localArgs.Length + args.Length];
                    Array.Copy(localArgs, newArgs, localArgs.Length);
                    Array.Copy(args, 0, newArgs, localArgs.Length, args.Length);
                    return eventBehaivour.SubscribeCondition(method, newArgs);
                };
            }
            else
                GameEvent += method.DynamicInvoke;
        }

        public virtual void Unsubcribe<TDel>(TDel method, params object[] args) where TDel : Delegate
        {
            if (eventBehaivour != null)
                // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                GameEvent -= (localArgs) =>
                {
                    object[] newArgs = new object[localArgs.Length + args.Length];
                    Array.Copy(localArgs, newArgs, localArgs.Length);
                    Array.Copy(args, 0, newArgs, localArgs.Length, args.Length);
                    return eventBehaivour.SubscribeCondition(method, newArgs);
                };
            else
                GameEvent -= method.DynamicInvoke;
        }
    }
}