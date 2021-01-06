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
        public UltEvent<Collider> onInteractCallback;
        
        private bool _hoverEnterFlag = false;
        private bool _hoverExitFlag = false;

        public InteractionInput InputType => interactionInputType;
        public InteractionInput interactionInputType = InteractionInput.Hold;
        
        public UltEvent<Collider> onHoverEnterCallback;
        public UltEvent<Collider> onHoverStayCallback;
        public UltEvent<Collider> onHoverExitCallback;
        public Collider LatestInteractor { get; private set; }

        public virtual void OnInteract(Collider collider)
        {
            onInteractCallback?.Invoke(collider);
            LatestInteractor = collider;
        }

        public void OnHoverEnter(Collider collider)
        {
            if (!_hoverEnterFlag)
            {
                _hoverEnterFlag = true;
                onHoverEnterCallback?.Invoke(collider);
                _hoverExitFlag = false;
            }
        }

        public void OnHoverStay(Collider collider)
        {
            onHoverStayCallback?.Invoke(collider);
        }

        public void OnHoverExit(Collider collider)
        {
            if (!_hoverExitFlag)
            {
                _hoverExitFlag = true;
                onHoverExitCallback?.Invoke(collider);
                _hoverEnterFlag = false;
            }
        }
    }
}