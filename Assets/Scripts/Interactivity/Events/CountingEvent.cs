using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public delegate void EntityEvent(Collider collider);

    [CreateAssetMenu(fileName = "New Count Event", menuName = "Event/Counting Event", order = 0)]
    public class CountingEvent : CustomEvent
    {
        public int maxAmount;
        private int _currentAmount;
        public GameObject targetObject;
        public bool resetOnCompletion;
        protected event EntityEvent EntityEvent;
        public bool showDebuglogs;

        private void OnEnable()
        {
            _currentAmount = 0;
        }

        public override void OnInvokeEvent()
        {
        }

        public void OnInvokeEvent(Collider collider = null)
        {
            if (collider == null) return;
            bool containsName = collider.gameObject.name.Contains(targetObject.name);
            if (showDebuglogs)
                Debug.Log(
                    $"{collider.gameObject.name}{(containsName ? " matches with" : " doesnt match with ")} {targetObject.name}");
            if (containsName)
            {
                _currentAmount++;
                _currentAmount = Mathf.Clamp(_currentAmount, 0, maxAmount);
                if (showDebuglogs) Debug.Log($"{maxAmount}/{_currentAmount}");
                if (_currentAmount.Equals(maxAmount))
                {
                    IsBeingCalled = true;
                    EntityEvent?.DynamicInvoke(collider);
                    if (resetOnCompletion)
                        _currentAmount = 0;
                }
            }

            IsBeingCalled = false;
        }

        public override void Subscribe<TDel>(TDel method)
        {
            EntityEvent += collider => method.DynamicInvoke(collider);
        }

        public override void Unsubcribe<TDel>(TDel method)
        {
            // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
            EntityEvent -= collider => method.DynamicInvoke(collider);
        }
    }
}