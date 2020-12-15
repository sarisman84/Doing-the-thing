using System;
using System.Collections.Generic;
using Extensions;
using Interactivity.Events;
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




        public InteractionInput interactionInputType = InteractionInput.Hold;

        public virtual void OnInteract(Collider collider)
        {
            onInteractCallback?.Invoke(collider);
        }
    }
}