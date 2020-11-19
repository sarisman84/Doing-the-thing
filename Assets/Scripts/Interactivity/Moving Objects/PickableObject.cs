using Player;
using UnityEngine;

namespace Interactivity.Moving_Objects
{
    public class PickableObject : MonoBehaviour, IInteractable
    {
        private Rigidbody _rigidbody;
        private Outline _outline;
        private bool _isBeingGrabbed;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }

        private void Update()
        {
            _rigidbody.useGravity = !_isBeingGrabbed;
            _isBeingGrabbed = false;
        }

        public void OnInteract(PlayerController controller)
        {
            _isBeingGrabbed = true;
            var pickupPosition = controller.transform.position + controller.playerCamera.transform.forward.normalized * 2f;
            _rigidbody.MovePosition(pickupPosition);
        }

        public void OnProximityEnter()
        {
            _outline.enabled = true;
        }

        public void OnProximityExit()
        {
            _outline.enabled = false;
        }

        public InteractionInput InputType { get; } = InteractionInput.Hold;

        public bool NeedToLookAtInteractable
        {
            get => !_isBeingGrabbed;
        }
    }
}