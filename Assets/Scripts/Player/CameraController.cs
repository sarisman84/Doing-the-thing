using System;
using Cinemachine;
using Interactivity.Events;
using UnityEngine;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        //Camera sensitivity
        [Range(1, 20f)] public float sensitivity = 200f;

        //Public reference to a Cinemachine Camera that is used for the player.
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        private PlayerController _playerController;
        private CinemachinePOV _pov;
        private Camera _mainCamera;
        public bool CameraLocked { get; set; }

        public float fieldOfView = 90f;
        public float fieldOfViewWhileSprinting = 100f;


        #region Global Methods
        private static CustomEvent onLockMouseCursor;

        public static void SetCursorActive(GameObject targetPlayer, bool state)
        {
            if (onLockMouseCursor)
                onLockMouseCursor.OnInvokeEvent(targetPlayer, state);
        }
        

        private void OnEnable()
        {
            onLockMouseCursor = CustomEvent.CreateEvent<Action<bool>>(AlterCursorState, gameObject);
        }

        private void OnDisable()
        {
            onLockMouseCursor.RemoveEvent<Action<bool>>(AlterCursorState);
        }
        #endregion

        public float CameraHeight
        {
            set
            {
                Vector3 newPos = playerCamera.Follow.localPosition;
                newPos.y = value;
                playerCamera.Follow.localPosition = newPos;
            }
            get { return playerCamera.Follow.localPosition.y; }
        }

        public Transform PlayerCamera
        {
            get => _mainCamera.transform;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _playerController = GetComponent<PlayerController>();
            if (playerCamera)
                _pov = playerCamera.GetCinemachineComponent<CinemachinePOV>();
           
        }

        private void Update()
        {
            if (!CameraLocked)
            {
                if (_playerController != null)
                {
                    playerCamera.m_Lens.FieldOfView =
                        _playerController.IsSprinting
                            ? Mathf.Lerp(playerCamera.m_Lens.FieldOfView, fieldOfViewWhileSprinting, 0.25f)
                            : Mathf.Lerp(playerCamera.m_Lens.FieldOfView, fieldOfView, 0.25f);
                    CameraHeight = _playerController.IsCrouching
                        ? Mathf.Lerp(CameraHeight, -0.5f, 0.25f)
                        : Mathf.Lerp(CameraHeight, 0, 0.25f);
                }

                if (playerCamera)
                {
                    _pov.m_HorizontalAxis.m_MaxSpeed = sensitivity;
                    _pov.m_VerticalAxis.m_MaxSpeed = sensitivity;
                }

                var transform1 = _playerController.transform;
                Quaternion rotation = transform1.rotation;
                rotation = new Quaternion(rotation.x,
                    _mainCamera.transform.rotation.y, rotation.z,
                    rotation.w);
                transform1.rotation = rotation;
            }
            else
            {
                if (playerCamera)
                {
                    _pov.m_HorizontalAxis.m_MaxSpeed = 0;
                    _pov.m_VerticalAxis.m_MaxSpeed = 0;
                }
            }
        }

        private void AlterCursorState(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = value;
        }


     

     

     


      
    }
}