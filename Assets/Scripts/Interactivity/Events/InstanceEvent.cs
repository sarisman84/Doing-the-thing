using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactivity.Events
{
    public delegate void InstanceDelegate(int id);
    [CreateAssetMenu(fileName = "New Instance Event", menuName = "Event/Instance Event", order = 0)]
    public class InstanceEvent : CustomEvent
    {
        

        private event InstanceDelegate InstanceDelegate;

        public void OnInvokeEvent(GameObject gameObject)
        {
         Debug.Log("Instance event being called");
            InstanceDelegate?.Invoke(gameObject.GetInstanceID());
        }

        public override void Unsubcribe<TDel>(TDel method)
        {
            InstanceDelegate -= id => method.DynamicInvoke(id);
        }

        public override void Subscribe<TDel>(TDel method)
        {
            InstanceDelegate += id => method.DynamicInvoke(id);
        }
    }
}