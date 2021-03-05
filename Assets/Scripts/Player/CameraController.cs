using System;
using Cinemachine;
using UnityEngine;
using Utility;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public const string ChangeCursorState = "Camera_ChangeCursorState";
        public const string CameraFallBehaivourEvent = "Player_SetFallingBehaivour";
        public const string SetCursorActiveEvent = "Player_SetCursorActive";
        public const string ConstantlyLookTowardsThePlayerEvent = "PlayerFall_LookAtPlayer";

        public enum CameraBehaivour
        {
            Follow,
            Look,
            FollowAndLook
        }

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
            EventManager.TriggerEvent(ChangeCursorState, false);
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


        private void AddListenersToEventManager()
        {
            EventManager.AddListener<Action<bool>>(ChangeCursorState, AlterCursorState);
            EventManager.AddListener<Action<bool>>(SetCursorActiveEvent, SetCameraAndCursorActive);
            EventManager.AddListener<Action<CameraBehaivour>>(CameraFallBehaivourEvent,
                value => SetCameraBehaivour(playerCamera, transform, value));
            EventManager.AddListener<Action>(ConstantlyLookTowardsThePlayerEvent,
                () => RotateCameraTowards(playerCamera, transform));
        }


        private void RemoveListenersFromEventManager()
        {
            EventManager.RemoveListener<Action<bool>>(ChangeCursorState, AlterCursorState);
            EventManager.RemoveListener<Action<bool>>(SetCursorActiveEvent, SetCameraAndCursorActive);
            EventManager.RemoveListener<Action<CameraBehaivour>>(CameraFallBehaivourEvent,
                value => SetCameraBehaivour(playerCamera, transform, value));
            EventManager.RemoveListener<Action>(ConstantlyLookTowardsThePlayerEvent,
                () => RotateCameraTowards(playerCamera, transform));
        }

        private void SetCameraAndCursorActive(bool value)
        {
            CameraLocked = value;

            EventManager.TriggerEvent(ChangeCursorState, value);
            EventManager.TriggerEvent(InputListener.SetPlayerLookInputActiveState, !value);
        }


        private void OnDisable()
        {
            //In case this gameObject is disabled, it is required to disable the input references with it as to avoid any input errors.

            RemoveListenersFromEventManager();
        }


        private void OnEnable()
        {
            //In case this gameObject is enabled, the references assigned to the gameObject will be enabled with the gameObject so that the player can regain control of this gameObject.

            AddListenersToEventManager();
        }

        private void SetCameraBehaivour(CinemachineVirtualCamera cam, Transform target, CameraBehaivour value)
        {
            switch (value)
            {
                case CameraBehaivour.Follow:
                    cam.Follow = target;
                    cam.LookAt = null;
                    cam.transform.parent = target;
                    break;
                case CameraBehaivour.Look:
                    cam.Follow = null;
                    cam.LookAt = target;
                    cam.transform.parent = null;
                    break;
                case CameraBehaivour.FollowAndLook:
                    cam.Follow = target;
                    cam.LookAt = target;
                    cam.transform.parent = target;
                    break;
            }
        }

        private void RotateCameraTowards(CinemachineVirtualCamera camera, Transform target)
        {
            var cameraTransform = camera.transform;
            camera.transform.rotation = Quaternion.Lerp(cameraTransform.rotation,
                Quaternion.LookRotation(target.position - cameraTransform.position, Vector3.up), 0.25f);
        }
    }
}