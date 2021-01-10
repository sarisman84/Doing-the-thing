using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactivity.Events.Conditions
{
    [CreateAssetMenu(fileName = "New Instance Condition Asset", menuName = "Event/Conditions/Instance Condition", order = 0)]
    public class InstanceEventBehaivour : CustomEventBehaivour
    {
        private readonly Dictionary<int, bool> _instanceStatusDictionary =
            new Dictionary<int, bool>();
        

        public override bool IsMet(params object[] args)
        {
            return true;
        }

        public override TDel SubscribeCondition<TDel>(TDel method, params object[] args)
        {
            return new Func<object>(() => TryInvokeMethod(args[0] as GameObject, args[1] as GameObject, method, args[3] as object[])) as TDel;
            
        }

        public override TDel UnSubscribeCondition<TDel>(TDel method, params object[] args)
        {
            return new Func<object>(() => TryInvokeMethod(args[0] as GameObject, args[1] as GameObject, method, args[3] as object[])) as TDel;
        }

        private object TryInvokeMethod<TDel>(GameObject entity, GameObject targetEntity, TDel method, object[] args)
            where TDel : Delegate
        {
            object results = default;

            int key = targetEntity.GetInstanceID() + GetInstanceID();
            if (!_instanceStatusDictionary.ContainsKey(key))
                _instanceStatusDictionary.Add(key, false);
            if (_instanceStatusDictionary.ContainsKey(key))
                Debug.Log(
                    $"Current instance (of target:{targetEntity.name}) is {(_instanceStatusDictionary[key] ? "being called" : "not being called")}");
            if (entity == null) return null;
            var areEqual = targetEntity.GetInstanceID() == entity.GetInstanceID();
            if (areEqual)
            {
                _instanceStatusDictionary[key] = true;
                results = method.DynamicInvoke(args);
                if (!triggerOnce)
                    _instanceStatusDictionary[key] = false;
            }

            return results;
        }
    }
}