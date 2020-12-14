﻿using System;
using System.Reflection;
using System.Threading;
using Extensions;
using Interactivity.Components;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public delegate void EmptyEvent();


    public delegate object ObjectEvent<T>(params T[] args);

    [CreateAssetMenu(fileName = "New Event Asset", menuName = "Event/New Event", order = 0)]
    public class CustomEvent : ScriptableObject
    {
        public bool IsBeingCalled { get; protected set; }
        protected event ObjectEvent<object> GameEvent;


        public virtual void OnInvokeEvent()
        {
            IsBeingCalled = true;
            GameEvent?.Invoke();
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