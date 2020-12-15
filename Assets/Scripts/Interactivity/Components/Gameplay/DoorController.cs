using System;
using DG.Tweening;
using Interactivity.Events;
using UnityEngine;

namespace Interactivity.Components.Gameplay
{
    public class DoorController : MonoBehaviour
    {
        public float doorOpeningDuration;
        private Vector3 _doorClosedPosition;

        public Vector3 doorOpenPosition;

        public Vector3 DoorOpenPosition
        {
            get { return doorOpenPosition + transform.position; }
        }

        private void Awake()
        {
            _doorClosedPosition = transform.position;
        }

        public void OpenDoor(int entity)
        {
            Debug.Log($"Incoming entity: {entity} -> {gameObject.GetInstanceID()}");
            if (gameObject.GetInstanceID() == entity)
                transform.DOMove(DoorOpenPosition, doorOpeningDuration);
        }

        public void CloseDoor(int entity)
        {
            Debug.Log($"Incoming entity: {entity} -> {gameObject.GetInstanceID()}");
            if (gameObject.GetInstanceID() == entity)

                transform.DOMove(_doorClosedPosition, doorOpeningDuration);
        }
    }
}