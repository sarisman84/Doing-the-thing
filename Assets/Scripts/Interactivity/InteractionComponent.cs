using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Object = UnityEngine.Object;

namespace Interactivity
{
    public enum InteractionType
    {
        PureInteraction,
        Damage,
        Both
    }

    public class InteractionComponent : MonoBehaviour, IInteractable, IDamageable
    {
        public InteractionType interactionType = InteractionType.Both;
        public InteractionInput inputType = InteractionInput.Hold;
        public bool needsToBeLookedAtForInteractivity = true;
    
        public string eventManagerEventName = "Insert event name here";
        public Object[] eventManagerEventArgs;
        public string[] eventManagerEventStringArgs;
#if UNITY_EDITOR
        [HideInInspector] public bool useEventSystem;
#endif

        public UnityEvent onAwakeEvent;
        public UnityEvent onInteractionEvent;
        public UnityEvent onProximityEnterEvent, onProximityExitEvent;
        public UnityEvent onDamageEvent;

        private void Awake()
        {
            onAwakeEvent?.Invoke();
            gameObject.layer = 14;
        }

        public void OnInteract(PlayerController controller)
        {
            if (interactionType == InteractionType.Both || interactionType == InteractionType.PureInteraction)
            {
                if (Array.FindAll(eventManagerEventArgs, o => o != null).Length != 0)
                    foreach (var obj in eventManagerEventArgs)
                    {
                        EventManager.TriggerEvent(eventManagerEventName, obj);
                    }

                foreach (var stringArg in eventManagerEventStringArgs)
                {
                    EventManager.TriggerEvent(eventManagerEventName, stringArg);
                }

                onInteractionEvent?.Invoke();
            }
        }

        public void OnProximityEnter()
        {
            if (interactionType == InteractionType.Both || interactionType == InteractionType.PureInteraction)
                onProximityEnterEvent?.Invoke();
        }

        public void OnProximityExit()
        {
            if (interactionType == InteractionType.Both || interactionType == InteractionType.PureInteraction)
                onProximityExitEvent?.Invoke();
        }

        public InteractionInput InputType
        {
            get => inputType;
        }

        public bool NeedToLookAtInteractable
        {
            get => needsToBeLookedAtForInteractivity;
        }

        public void TakeDamage(float damage)
        {
            if (interactionType == InteractionType.Damage)
                onDamageEvent?.Invoke();
        }

        public bool IsDead => gameObject.activeSelf;
    }
}