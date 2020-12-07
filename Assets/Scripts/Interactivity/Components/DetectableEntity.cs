using System;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Attributes;
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

        [EnableIf("useCustomEvent")]
        public CustomEvent onAreaEnterCustomEvent, onAreaStayCustomEvent, onAreaExitCustomEvent;

        private void Awake()
        {
            if(executeOnExitCallbackAtStart)
                onAreaExit?.Invoke(null);
        }

        public void OnAreaEnter(Collider col)
        {
            onAreaEnter?.Invoke(col);
            TriggerMethod(onAreaEnterCustomEvent);
        }

        public void OnAreaStay(Collider col)
        {
            onAreaStay?.Invoke(col);
            TriggerMethod(onAreaStayCustomEvent);
        }

        public void OnAreaExit(Collider col)
        {
            onAreaExit?.Invoke(col);
            TriggerMethod(onAreaExitCustomEvent);
        }


        private void TriggerMethod(CustomEvent customEvent)
        {
            if (useCustomEvent)
                EventManager.TriggerEvent(customEvent.name, customEvent.args);
        }
    }

    [Serializable]
    public class CustomEvent
    {
        public string name;
        public Object[] args;
    }
}