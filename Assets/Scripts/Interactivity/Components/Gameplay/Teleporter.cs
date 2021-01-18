using System;
using UnityEngine;

namespace Interactivity.Components.Gameplay
{
    public class Teleporter : MonoBehaviour
    {
        public Teleporter linkedTeleporter;
        public Transform exitPosition;

        public bool activateOnAwake;
        public bool IsActive { get; set; }

        private void Awake()
        {
            IsActive = activateOnAwake;
        }

        public void Teleport(Collider col)
        {
            if (!IsActive || linkedTeleporter == null) return;
            col.transform.position = linkedTeleporter.exitPosition.position;
        }
    }
}
