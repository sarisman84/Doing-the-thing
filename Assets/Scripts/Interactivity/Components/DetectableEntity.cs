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
        public bool checkSelf;
        private Collider _collider;


        private void Awake()
        {
            _collider = GetComponent<Collider>();

            if (executeOnExitCallbackAtStart)
                onAreaExit?.Invoke(_collider ? _collider : null);
        }

        public void OnAreaEnter(Collider col)
        {
            onAreaEnter?.Invoke(checkSelf ? _collider : col);
        }

        public void OnAreaStay(Collider col)
        {
            onAreaStay?.Invoke(checkSelf ? _collider : col);
        }

        public void OnAreaExit(Collider col)
        {
            onAreaExit?.Invoke(checkSelf ? _collider : col);
        }
    }
}