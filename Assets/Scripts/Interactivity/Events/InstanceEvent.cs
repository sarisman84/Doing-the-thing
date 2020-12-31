using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactivity.Events.Listener;
using UnityEngine;

namespace Interactivity.Events
{
    public delegate void InstanceDelegate(GameObject id);

    [CreateAssetMenu(fileName = "New Instance Event", menuName = "Event/Instance Event", order = 0)]
    public class InstanceEvent : CustomEvent
    {
        private event InstanceDelegate InstanceDelegate;

        public readonly Dictionary<int, bool> instanceStatusDictionary =
            new Dictionary<int, bool>();


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
            if (instanceStatusDictionary.Count > 1)
                return instanceStatusDictionary.All(pair => pair.Value);
            return false;
        }


        public void OnInvokeEvent(GameObject gameObject)
        {
            InstanceDelegate?.Invoke(gameObject);
        }

        public void OnInvokeEvent(Collider collider)
        {
            OnInvokeEvent(collider.gameObject);
        }

        public override void Unsubcribe<TDel>(TDel method)
        {
            InstanceDelegate -= id => method.DynamicInvoke(id);
        }

        public override void Subscribe<TDel>(TDel method)
        {
            InstanceDelegate += id => method.DynamicInvoke(id);
        }


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
            if (!instanceEvent.instanceStatusDictionary.ContainsKey(targetEntity.GetInstanceID()))
                instanceEvent.instanceStatusDictionary.Add(targetEntity.GetInstanceID(), false);

            var areEqual = targetEntity.GetInstanceID() == entity.GetInstanceID();
            if (areEqual)
            {
                instanceEvent.instanceStatusDictionary[targetEntity.GetInstanceID()] = true;
                eventDefinition.InvokeEvent(entity);
                if (!instanceEvent.triggerOnce)
                    instanceEvent.instanceStatusDictionary[targetEntity.GetInstanceID()] = false;
                return true;
            }

            return false;
        }

        public static void InstanceCheck<TDel>(GameObject entity, InstanceEvent eventInfo, TDel method)
            where TDel : Delegate
        {
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

        public bool AreSomeCalled()
        {
            return instanceStatusDictionary.Any(pair => pair.Value);
        }
    }
}