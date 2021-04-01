using System;
using DG.Tweening;
using Interactivity.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using Utility.Attributes;

namespace Interactivity.Components.Gameplay
{
    public class DoorController : MonoBehaviour
    {
        public Mesh previewMesh;
        public float doorOpeningDuration;
        [VisualisePosition(true)] public Vector3 doorOpenPosition;


        public Vector3 DoorOpenPosition => doorOpenPosition + _doorClosedPosition;
        private Vector3 _doorClosedPosition;

        private void Awake()
        {
            _doorClosedPosition = transform.position;
        }

        public void OpenDoor()
        {
            //Debug.Log($"Incoming entity: {entity} -> {gameObject.GetInstanceID()}");
            //if (gameObject.GetInstanceID() == entity.GetInstanceID())
            transform.DOMove(DoorOpenPosition, doorOpeningDuration);
        }

        public void CloseDoor()
        {
            //Debug.Log($"Incoming entity: {entity} -> {gameObject.GetInstanceID()}");
            //if (gameObject.GetInstanceID() == entity.GetInstanceID())
            transform.DOMove(_doorClosedPosition, doorOpeningDuration);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            bool isSelected = Selection.activeGameObject &&
                              Selection.activeGameObject.GetInstanceID() == gameObject.GetInstanceID();
            Gizmos.color = isSelected
                ? Color.green
                : Color.green - new Color(0, 0, 0, 0.5f);

            Vector3 doorPos = Application.isPlaying ? DoorOpenPosition : transform.position + DoorOpenPosition;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.25f);

            Gizmos.color = isSelected ? Color.red : Color.red - new Color(0, 0, 0, 0.5f);

            Gizmos.DrawCube(doorPos, Vector3.one * 0.25f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Application.isPlaying ? _doorClosedPosition : transform.position, doorPos);

            if (!previewMesh)
            {
                Gizmos.matrix = Matrix4x4.TRS(doorPos, transform.rotation, transform.lossyScale);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
            else
            {
                Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);
                Gizmos.DrawMesh(previewMesh, doorPos);
            }
        }

#endif
    }
}