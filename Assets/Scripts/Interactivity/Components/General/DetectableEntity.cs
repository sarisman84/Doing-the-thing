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
    public class DetectableEntity : MonoBehaviour
    {
        public UnityEvent<Collider> onAreaEnter;
        public UnityEvent<Collider> onAreaStay;
        public UnityEvent<Collider> onAreaExit;

        public bool executeOnExitCallbackAtStart;
        public bool checkSelf;
        private Collider _collider;

        private Collider _spotter;
        private float _distFromSpotter, _previousDistFromSpotter;


        private void Awake()
        {
            _collider = GetComponent<Collider>();

            if (executeOnExitCallbackAtStart)
                onAreaExit?.Invoke(_collider ? _collider : null);
        }

        private void Update()
        {
            if (_spotter)
            {
                _previousDistFromSpotter = _distFromSpotter;
                _distFromSpotter = (_spotter.transform.position - transform.position).magnitude;
            }
        }

        private void OnAreaEnter(Collider col)
        {
            onAreaEnter?.Invoke(checkSelf ? _collider == null ? null : _collider : col);
        }

        private void OnAreaStay(Collider col)
        {
            onAreaStay?.Invoke(checkSelf ? _collider == null ? null : _collider : col);
        }

        private void OnAreaExit(Collider col)
        {
            onAreaExit?.Invoke(checkSelf ? _collider == null ? null : _collider : col);
        }

        public void OnDetect(Collider col, float overlapSphereRange)
        {
            //If a spotter was assigned before and the distance between the spotter and this object in the last frame is less than the current frame while being equal or higher distance in relation to the spotter,
            //it is safe to assume that we just exited the spotter's detection area.
            if (_spotter && _previousDistFromSpotter < _distFromSpotter && overlapSphereRange <= _distFromSpotter)
            {
                OnAreaExit(col);
                _spotter = null;
                _distFromSpotter = 0;
            }

            //If a spotter was not assigned before, it means that we just got detected.
            //Therefore, it is safe to assume that we just entered the spotter's detection area.
            if (!_spotter)
            {
                OnAreaEnter(col);
                _spotter = col;
            }

            //If this method is called, it means that we are within the spotter's detection area.
            OnAreaStay(col);
        }
    }
}