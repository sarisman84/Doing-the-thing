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

        public ObjectEvent<object> CurrentEvent => GameEvent;


        public object OnInvokeEvent(GameObject gameObject, params object[] args)
        {
            return GameEvent?.Invoke(gameObject, args);
        }

        public void OnInvokeEvent(Collider collider)
        {
            OnInvokeEvent(collider.gameObject);
        }

        public void OnInvokeEvent(GameObject gameObject)
        {
            OnInvokeEvent(gameObject, null);
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

        public static CustomEvent CreateEvent<TDel>(TDel method,
            GameObject instanceCondition = null)
            where TDel : Delegate
        {
            CustomEvent customEvent = CreateInstance<CustomEvent>();
            customEvent.Subscribe(method, instanceCondition);
            return customEvent;
        }
    }


    public static class EventExtensions
    {
        public static void RemoveEvent<TDel>(this CustomEvent customEvent, TDel method) where TDel : Delegate
        {
            if (customEvent)
                customEvent.Unsubcribe(method);
            customEvent = null;
        }
    }
}