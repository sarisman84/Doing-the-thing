using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactivity.Events.Conditions
{
    public abstract class CustomEventBehaivour : ScriptableObject
    {
        public bool triggerOnce;
        protected bool isConditionMet = false;
        public bool IsConditionMet => isConditionMet;
        public abstract bool IsMet(params object[] args);
        public abstract TDel SubscribeCondition<TDel>(TDel method, params object[] args) where TDel : Delegate;
        public abstract TDel UnSubscribeCondition<TDel>(TDel method, params object[] args) where TDel : Delegate;

        public virtual void OnEnable()
        {
            isConditionMet = false;
        }
    }

    
}