using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Utility.TestScripts
{
    public class GroupMockup : MonoBehaviour
    {
        #region Custom Event Template

        public bool isEventGlobal;

        public void OnInvokeEvent()
        {
        }

        public void Subscribe<TDel>(TDel method, GameObject instanceCondition = null) where TDel : Delegate
        {
            eventsToListenTo.ApplyAction(e => { GroupInvoke(method, instanceCondition, e.customEvent); });
        }

        public void Unsubscribe<TDel>(TDel method, GameObject instanceCondition = null) where TDel : Delegate
        {
            eventsToListenTo.ApplyAction(e => { GroupInvoke(method, instanceCondition, e.customEvent, false); });
        }

        #endregion

        #region GroupEvent Logic

        public EventElementStruct[] eventsToListenTo;

        private object TryInvokeOnCondition<TDel>(TDel method) where TDel : Delegate
        {
            //if(eventsToListenTo.All(e => e.IsEventBeingCalled))
            /*  return TryInvokeMethod(method);
             */

            return default;
        }

        private void GroupInvoke<TDel>(TDel method, GameObject instanceCondition, Interactivity.Events.CustomEvent e,
            bool subscribe = true) where TDel : Delegate
        {
            if (subscribe)
            {
                e.Subscribe<Func<object>>(() => TryInvokeOnCondition(method),
                    isEventGlobal ? null : instanceCondition);
                return;
            }

            e.Unsubcribe<Func<object>>(() => TryInvokeOnCondition(method), isEventGlobal ? null : instanceCondition);
        }

        #endregion
    }


    [Serializable]
    public struct EventElementStruct
    {
        public Interactivity.Events.CustomEvent customEvent;
        public List<GameObject> instanceConditions;
    }
}