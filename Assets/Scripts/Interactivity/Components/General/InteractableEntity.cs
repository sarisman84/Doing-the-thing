using System;
using System.Collections.Generic;
using Extensions;
using Interactivity.Events;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Utility.Attributes;
using Object = UnityEngine.Object;

namespace Interactivity.Components
{
    public class InteractableEntity : MonoBehaviour, IInteractable
    {
        public UltEvent<GameObject> onAwakeCallback;
        public UltEvent<GameObject,Collider> onInteractCallback;
        
        private bool _hoverEnterFlag = false;
        private bool _hoverExitFlag = false;

        public InteractionInput InputType => interactionInputType;
        public InteractionInput interactionInputType = InteractionInput.Hold;
        
        public UltEvent<GameObject,Collider> onHoverEnterCallback;
        public UltEvent<GameObject,Collider> onHoverStayCallback;
        public UltEvent<GameObject,Collider> onHoverExitCallback;
        public Collider LatestInteractor { get; private set; }

        private void Awake()
        {
            onAwakeCallback?.Invoke(gameObject);
        }

        public virtual void OnInteract(Collider collider)
        {
            onInteractCallback?.Invoke(gameObject,collider);
            LatestInteractor = collider;
        }

        public void OnHoverEnter(Collider collider)
        {
            if (!_hoverEnterFlag)
            {
                _hoverEnterFlag = true;
                onHoverEnterCallback?.Invoke(gameObject,collider);
                _hoverExitFlag = false;
            }
        }

        public void OnHoverStay(Collider collider)
        {
            onHoverStayCallback?.Invoke(gameObject,collider);
        }

        public void OnHoverExit(Collider collider)
        {
            if (!_hoverExitFlag)
            {
                _hoverExitFlag = true;
                onHoverExitCallback?.Invoke(gameObject,collider);
                _hoverEnterFlag = false;
            }
        }
    }
}