using System;
using UnityEngine;

namespace Interactivity.Events
{
    public delegate object ObjectEvent<in T>(GameObject gameObject = null, params T[] args);

    [CreateAssetMenu(fileName = "New Event Asset", menuName = "Event/New Event", order = 0)]
    public class CustomEvent : ScriptableObject
    {
        protected event ObjectEvent<object> GameEvent;
        public bool isEventGlobal;
        public bool showDebugMessages;


        public void OnInvokeEvent(GameObject gameObject)
        {
            OnInvokeEvent(gameObject, null);
        }

        public object OnInvokeEvent(GameObject gameObject, params object[] args)
        {
            if (isEventGlobal)
            {
                return GameEvent?.Invoke(gameObject, args);
            }

            return GameEvent?.Invoke(gameObject, args);
        }


        public void Subscribe<TDel>(TDel method, GameObject instanceCondition = null)
            where TDel : Delegate
        {
            GameEvent += (gameObject, args) => OnGameEvent(method, gameObject, instanceCondition, args);
        }

        private object OnGameEvent<TDel>(TDel method, GameObject gameObject, GameObject instanceGameObject,
            object[] args) where TDel : Delegate
        {
            if (!isEventGlobal && (instanceGameObject != null || gameObject != null))
            {
                return TryInvokeMethod(gameObject, instanceGameObject, method, args);
            }

            return method.DynamicInvoke(args);
        }

        public void Unsubcribe<TDel>(TDel method, GameObject instanceCondition = null)
            where TDel : Delegate
        {
            // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
            GameEvent -= (gameObject, args) => OnGameEvent(method, gameObject, instanceCondition, args);
        }


        protected object TryInvokeMethod<TDel>(GameObject entity, GameObject targetEntity, TDel method,
            object[] args)
            where TDel : Delegate
        {
            object results = default;
            if (entity == null)
            {
                return method.DynamicInvoke(args);
            }

            var areEqual = targetEntity.GetInstanceID() == entity.GetInstanceID();
            if (areEqual)
            {
                if (showDebugMessages)
                    Debug.Log($"Calling event to object: {targetEntity.name}");
                results = method.DynamicInvoke(args);
            }

            return results;
        }

        public static CustomEvent CreateEvent<TDel>(ref CustomEvent customEvent, TDel method,
            GameObject instanceCondition = null)
            where TDel : Delegate
        {
            customEvent = customEvent ? customEvent : CreateInstance<CustomEvent>();
            customEvent.Subscribe(method, instanceCondition);
            return customEvent;
        }
    }
}