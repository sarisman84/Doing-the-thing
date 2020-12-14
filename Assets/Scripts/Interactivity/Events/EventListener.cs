using System;
using System.Collections.Generic;
using Extensions;
using Player;
using Player.Weapons;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Interactivity.Events
{
    public class EventListener : MonoBehaviour
    {
        public List<EventInfo> events = new List<EventInfo>();

        private void OnEnable()
        {
            events.ApplyAction(e => e.AddResponses());
        }

        private void OnDisable()
        {
            events.ApplyAction(e => e.RemoveResponses());
        }

        private void OnDestroy()
        {
            events.ApplyAction(e => e.RemoveResponses());
        }
    }

    [Serializable]
    public class EventInfo
    {
        public CustomEvent listener;

        [Space] public UnityEvents unityResponse;
        public CustomEvent customResponse;

        public void AddResponses()
        {
            switch (listener)
            {
                case CountingEvent countingListener:
                    countingListener.Subscribe<Action<Collider>>((col) => unityResponse.InvokeEvent(col));
                    break;

                default:

                    listener.Subscribe<Action>(() => unityResponse.InvokeEvent());
                    break;
            }

            if (customResponse)
                listener.Subscribe<Action>(() => customResponse.OnInvokeEvent());
        }

        public void RemoveResponses()
        {
            if (listener is CountingEvent countingEvent)
            {
                countingEvent.Subscribe<Action<Collider>>((col) => unityResponse.InvokeEvent(col));
            }
            else
                listener.Unsubcribe<Action>(() => unityResponse.InvokeEvent());

            if (customResponse)
                listener.Unsubcribe<Action>(() => customResponse.OnInvokeEvent());
        }
    }

    [Serializable]
    public class UnityEvents
    {
        public EventType eventType;
        public UnityEvent parameterlessEvent;

        public Color colorArg;
        public UnityEvent<Color> colorEvent;

        public int intArg;
        public UnityEvent<int> intEvent;

        public float floatArg;
        public UnityEvent<float> floatEvent;

        public Collider colliderArg;
        public UnityEvent<Collider> colliderEvent;

        public GameObject gameObjectArg;
        public UnityEvent<GameObject> gameObjectEvent;

        public string stringArg;
        public UnityEvent<string> stringEvent;

        public WeaponController weaponLibraryArg;
        public UnityEvent<List<Weapon>> weaponLibraryEvent;

        public bool useExtraArgs;

        public UnityEvent<int> DelegateArg;
        public UnityEvent<Action<int>, List<Weapon>> extraWeaponLibraryEvent;


        public enum EventType
        {
            Int,
            Float,
            String,
            Color,
            Collider,
            GameObject,
            WeaponLibrary,
            Void
        }

        public void InvokeEvent(object arg = null)
        {
            switch (eventType)
            {
                case EventType.Int:
                    intEvent.Invoke(arg is int i ? i : intArg);
                    break;
                case EventType.Float:
                    floatEvent.Invoke(arg is float f ? f : floatArg);
                    break;
                case EventType.String:
                    stringEvent.Invoke(arg is string s ? s : stringArg);
                    break;
                case EventType.Color:
                    colorEvent.Invoke(arg is Color color ? color : colorArg);
                    break;
                case EventType.Collider:
                    colliderEvent.Invoke(arg is Collider collider ? collider : colliderArg);
                    break;
                case EventType.GameObject:
                    gameObjectEvent.Invoke(arg is GameObject gameObject ? gameObject : gameObjectArg);
                    break;
                case EventType.Void:
                    parameterlessEvent.Invoke();
                    break;
                case EventType.WeaponLibrary:
                    if (useExtraArgs)
                        extraWeaponLibraryEvent.Invoke((integer) => DelegateArg.Invoke(integer), weaponLibraryArg.weaponLibrary);
                    else
                        weaponLibraryEvent.Invoke(weaponLibraryArg.weaponLibrary);
                    break;
            }
        }
    }
}