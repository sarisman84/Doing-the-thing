using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactivity.Events.Listener;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public delegate object InstanceDelegate(GameObject id, object[] args = null);

    [CreateAssetMenu(fileName = "New Instance Event", menuName = "Event/Instance Event", order = 0)]
    public class InstanceEvent : CustomEvent
    {
        private event InstanceDelegate InstanceDelegate;
#if UNITY_EDITOR
        public bool showDebugMessages;
#endif
        public readonly Dictionary<int, bool> instanceStatusDictionary =
            new Dictionary<int, bool>();

        public List<bool> CurrentStatus => instanceStatusDictionary.Values.ToList();


        private void OnDisable()
        {
            instanceStatusDictionary.Clear();
        }

        /// <summary>
        /// Checks whenever all known instances of this event are being called.
        /// </summary>
        /// <returns>The overall state of this event.</returns>
        public bool AreAllInstancesBeingCalled()
        {
            if (instanceStatusDictionary.Keys.Count != 0)
                return instanceStatusDictionary.All(pair => pair.Value);
            return false;
        }

        public bool AreSomeCalled()
        {
            return instanceStatusDictionary.Any(pair => pair.Value);
        }


        public void OnInvokeEvent(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (showDebugMessages)
                Debug.Log($"Calling {name}. Target object is {gameObject.name}");
#endif
            InstanceDelegate?.Invoke(gameObject);
        }

        public void OnInvokeEvent(GameObject gameObject, params object[] args)
        {
            InstanceDelegate?.Invoke(gameObject, args);
        }

        public void OnInvokeEvent(Collider collider)
        {
            OnInvokeEvent(collider.gameObject);
        }

        public object OnInvokeEventAndGetResult(GameObject gameObject)
        {
            return InstanceDelegate?.Invoke(gameObject);
        }


        public override void Unsubcribe<TDel>(TDel method)
        {
            InstanceDelegate -= (id, args) => method.DynamicInvoke(id);
        }

        public override void Subscribe<TDel>(TDel method)
        {
            InstanceDelegate += (id, args) => method.DynamicInvoke(id);
        }

        public void Unsubcribe<TDel>(TDel method, GameObject _id) where TDel : Delegate
        {
            InstanceDelegate -= (id, objects) => { return TryInvokeMethod(id, _id, method, objects); };
        }


        public void Subscribe<TDel>(TDel method, GameObject _id) where TDel : Delegate
        {
            InstanceDelegate += (id, objects) => { return TryInvokeMethod(id, _id, method, objects); };
        }

        private object TryInvokeMethod<TDel>(GameObject entity, GameObject targetEntity, TDel method, object[] args)
            where TDel : Delegate
        {
            object results = default;

            int key = targetEntity.GetInstanceID() + GetInstanceID();
            if (!instanceStatusDictionary.ContainsKey(key))
                instanceStatusDictionary.Add(key, false);
            if (instanceStatusDictionary.ContainsKey(key))
                Debug.Log(
                    $"Current instance (of target:{targetEntity.name}) is {(instanceStatusDictionary[key] ? "being called" : "not being called")}");
            if (entity == null) return null;
            var areEqual = targetEntity.GetInstanceID() == entity.GetInstanceID();
            if (areEqual)
            {
                instanceStatusDictionary[key] = true;
                results = method.DynamicInvoke(args);
                if (!triggerOnce)
                    instanceStatusDictionary[key] = false;
            }

            return results;
        }

        #region Static Methods

        public static void InstanceCheck(GameObject entity, EventDefinition eventDefinition,
            InstanceEvent instanceEvent)
        {
            if (!eventDefinition.useCustomArg)
            {
                eventDefinition.InvokeEvent(entity);
                return;
            }

            var interactable = eventDefinition.Owner.GetComponent<IInteractable>();
            if (eventDefinition.useInteractor && interactable != null)
            {
                if (TryInvokeMethod(entity, eventDefinition, instanceEvent,
                    interactable.LatestInteractor.gameObject)) return;
            }


            foreach (GameObject targetEntity in eventDefinition.entityComparison)
            {
                if (TryInvokeMethod(entity, eventDefinition, instanceEvent, targetEntity)) break;
            }
        }

        private static bool TryInvokeMethod(GameObject entity, EventDefinition eventDefinition,
            InstanceEvent instanceEvent,
            GameObject targetEntity)
        {
            int key = targetEntity.GetInstanceID() + instanceEvent.GetInstanceID();
            if (!instanceEvent.instanceStatusDictionary.ContainsKey(key))
                instanceEvent.instanceStatusDictionary.Add(key, false);

            var areEqual = targetEntity.GetInstanceID() == entity.GetInstanceID();
            if (areEqual)
            {
                instanceEvent.instanceStatusDictionary[key] = true;
                eventDefinition.InvokeEvent(entity);
                if (!instanceEvent.triggerOnce)
                    instanceEvent.instanceStatusDictionary[key] = false;
                return true;
            }

            return false;
        }

        public static void InstanceCheck<TDel>(GameObject entity, InstanceEvent eventInfo, TDel method)
            where TDel : Delegate
        {
            if (!eventInfo.instanceStatusDictionary.ContainsKey(entity.GetInstanceID()))
                eventInfo.instanceStatusDictionary.Add(entity.GetInstanceID(), false);

            foreach (var targetEntity in eventInfo.instanceStatusDictionary)
            {
                var areEqual = targetEntity.Key == entity.GetInstanceID();
                if (areEqual)
                {
                    eventInfo.instanceStatusDictionary[targetEntity.Key] = true;
                    method.DynamicInvoke();
                    if (!eventInfo.triggerOnce)
                        eventInfo.instanceStatusDictionary[targetEntity.Key] = false;
                    break;
                }
            }
        }

        #endregion

        public static void CreateEvent<TDel>(ref InstanceEvent instanceEvent, GameObject owner, TDel openShop)
            where TDel : Delegate
        {
            instanceEvent = instanceEvent ? instanceEvent : CreateInstance<InstanceEvent>();
            instanceEvent.Subscribe(openShop, owner);
        }
    }
}