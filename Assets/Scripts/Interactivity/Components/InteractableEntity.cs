using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Attributes;
using Object = UnityEngine.Object;

namespace Interactivity.Components
{
    public class InteractableEntity : MonoBehaviour, IInteractable
    {
        public UnityEvent<Collider> onInteractCallback;
        public InteractionInput InputType => interactionInputType;
        public bool useCustomEvent;

        [EnableIf("useCustomEvent")] public string eventName;
        [EnableIf("useCustomEvent")] public Object[] args;


        private readonly List<Action<Collider>> _storedCallbacks = new List<Action<Collider>>();

        public InteractionInput interactionInputType = InteractionInput.Hold;

        public virtual void OnInteract(Collider collider)
        {
            onInteractCallback?.Invoke(collider);
            if (useCustomEvent)
            {
                EventManager.TriggerEvent(eventName, args);
            }
        }


        public IInteractable AddCallback(Action<Collider> callback)
        {
            _storedCallbacks.Add(callback);
            onInteractCallback.AddListener(callback.Invoke);
            return this;
        }


        private void OnEnable()
        {
            AddStoredCallbacks();
        }

        private void OnDisable()
        {
            RemoveStoredCallbacks();
        }

        private void RemoveStoredCallbacks()
        {
            _storedCallbacks.ApplyAction(c =>   onInteractCallback.RemoveListener(c.Invoke));

        }

        private void AddStoredCallbacks()
        {
            _storedCallbacks.ApplyAction(c =>   onInteractCallback.AddListener(c.Invoke));
        }
    }
}