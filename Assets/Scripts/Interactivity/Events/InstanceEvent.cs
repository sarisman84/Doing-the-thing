using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactivity.Events
{
    public delegate bool InstanceDelegate(GameObject id);

    [CreateAssetMenu(fileName = "New Instance Event", menuName = "Event/Instance Event", order = 0)]
    public class InstanceEvent : CustomEvent
    {
        private event InstanceDelegate InstanceDelegate;
        public bool IsInstanceBeingCalled { get; private set; }

        public void OnInvokeEvent(GameObject gameObject)
        {
            if (InstanceDelegate != null)
                IsInstanceBeingCalled = InstanceDelegate.Invoke(gameObject);
            IsInstanceBeingCalled = !triggerOnce ? false : IsInstanceBeingCalled;
        }

        public override void Unsubcribe<TDel>(TDel method)
        {
            InstanceDelegate -= id => (bool) method.DynamicInvoke(id);
        }

        public override void Subscribe<TDel>(TDel method)
        {
            InstanceDelegate += id => (bool) method.DynamicInvoke(id);
        }
    }
}