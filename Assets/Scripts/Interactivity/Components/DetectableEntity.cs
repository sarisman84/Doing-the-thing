using System;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Attributes;
using CustomEvent = Interactivity.Events.CustomEvent;
using Object = UnityEngine.Object;

namespace Interactivity.Components
{
    public class DetectableEntity : MonoBehaviour, IDetectable
    {
        public UnityEvent<Collider> onAreaEnter;
        public UnityEvent<Collider> onAreaStay;
        public UnityEvent<Collider> onAreaExit;

        public bool executeOnExitCallbackAtStart;
        public bool useCustomEvent;

        [EnableIf("useCustomEvent")] public CustomEventWrapper onAreaEnterCustomEventWrapper,
            onAreaStayCustomEventWrapper,
            onAreaExitCustomEventWrapper;

        private void Awake()
        {
            if (executeOnExitCallbackAtStart)
                onAreaExit?.Invoke(null);
        }

        public void OnAreaEnter(Collider col)
        {
            onAreaEnter?.Invoke(col);
            TriggerMethod(onAreaEnterCustomEventWrapper);
        }

        public void OnAreaStay(Collider col)
        {
            onAreaStay?.Invoke(col);
            TriggerMethod(onAreaStayCustomEventWrapper);
        }

        public void OnAreaExit(Collider col)
        {
            onAreaExit?.Invoke(col);
            TriggerMethod(onAreaExitCustomEventWrapper);
        }


        private void TriggerMethod(CustomEventWrapper customEventWrapper)
        {
            if (!customEventWrapper.customEvent)
                customEventWrapper.customEvent.Raise(customEventWrapper.triggerOnce);
        }
    }

    [Serializable]
    public class CustomEventWrapper
    {
        public CustomEvent customEvent;
        public bool triggerOnce;
    }
}