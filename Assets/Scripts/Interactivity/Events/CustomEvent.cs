using System;
using System.Reflection;
using System.Threading;
using Extensions;
using Interactivity.Components;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public delegate void EmptyEvent();


    public delegate object ObjectEvent<in T>(params T[] args);

    [CreateAssetMenu(fileName = "New Event Asset", menuName = "Event/Default Event", order = 0)]
    public class CustomEvent : ScriptableObject
    {
        public bool triggerOnce = false;
        public bool IsBeingCalled { get; protected set; }
        protected event ObjectEvent<object> GameEvent;

        protected virtual void OnEnable()
        {
            IsBeingCalled = false;
        }

        public virtual void OnInvokeEvent()
        {
            if (!IsBeingCalled)
                GameEvent?.Invoke();
            IsBeingCalled = true;

            if (!triggerOnce)
                IsBeingCalled = false;
        }

        public virtual void OnInvokeEnter(WeaponController controller)
        {
            if (!IsBeingCalled)
                GameEvent?.Invoke(controller);
            IsBeingCalled = true;

            if (!triggerOnce)
                IsBeingCalled = false;
        }
        


        public virtual void Subscribe<TDel>(TDel method) where TDel : Delegate
        {
            GameEvent += method.DynamicInvoke;
        }

        public virtual void Unsubcribe<TDel>(TDel method) where TDel : Delegate
        {
            GameEvent -= method.DynamicInvoke;
        }
    }
}