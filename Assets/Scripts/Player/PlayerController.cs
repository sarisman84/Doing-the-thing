using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Pickup;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using static Player.InputListener.KeyCode;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public const string CameraFallBehaivourEvent = "Player_SetFallingBehaivour";
        public const string SetCursorActiveEvent = "Player_SetCursorActive";

        public const string ConstantlyLookTowardsThePlayerEvent = "PlayerFall_LookAtPlayer";
        public const string MoveEntityEvent = "Player_MoveEntity";


        //Controller information

        #region Controller Variables

        //Movement
        [Space] public float movementSpeed;
        public float sprintMultiplier;
        public float crouchMultiplier;

        //Camera sensitivity
        [Range(50, 300f)] public float sensitivity = 200f;
        public float jumpForce;

        //Collision sizes and checks for the Ground and Stand methods.
        public Vector3 groundCheckSize = Vector3.one;
        public Vector3 sealingCheckSize = Vector3.one;
        public LayerMask movementCheckLayer;

        //Public reference to a Cinemachine Camera that is used for the player.
        public CinemachineFreeLook playerCamera;

        //FOV information for both normal and sprint modes.
        [Range(10f, 100f)] public float fieldOfView = 60f;
        [Range(10f, 100f)] public float fieldOfViewWhileSprinting = 90f;

        //Pickup information


        //Local references to components and custom classes. 
        //Local variables that store information such as speed or input.
        private Rigidbody _physics;
        private CapsuleCollider _collisionBody;
        public FPCameraHandler fpcHandler;
        public WeaponController weaponController;
        private Vector2 _inputVector;
        private float _totalSpeed;
        private float _standingHeight;
        private Vector3 _trueInputVector;
        private Vector2 _lookValue;

        private float _originalFOV;
        private float _sprintFOV;
        private bool _isSprinting;
        private bool _isJumping;
        private bool _isCrouching;
        private bool _isInteracting;

        #endregion


        public bool CameraLocked { get; set; }
        public event Action ONUpdateCallback;

        private Vector3 BottonPositionOfCollider
        {
            get
            {
                var position = transform.position;
                return new Vector3(position.x, position.y - (_collisionBody.bounds.size.y / 2f), position.z);
            }
        }

        private Vector3 TopPositionOfCollider
        {
            get
            {
                var position = transform.position;
                return new Vector3(position.x, position.y + (_collisionBody.bounds.size.y / 2f), position.z);
            }
        }

        private void Awake()
        {
            //Assigns the player as a priority target for any enemy.
            EnemyBehaivourManager.AssignNewTarget(transform);

            //Init
            _collisionBody = GetComponent<CapsuleCollider>();
            _physics = GetComponent<Rigidbody>();
            fpcHandler = new FPCameraHandler();

            //Saving values for FOV and Sprint FOV respectively.
            _originalFOV = fieldOfView;
            _sprintFOV = fieldOfViewWhileSprinting;

            //Applies FOV to the camera's lens.
            playerCamera.m_Lens.FieldOfView = _originalFOV;


            //Init for the weapon managing class that handles the selection and firing of the current weapon.
            weaponController = new WeaponController(this);
        }


        private void Update()
        {
            //Get values from Input References
            _inputVector = InputListener.GetAxisRaw();
            _lookValue = InputListener.GetMouseDelta();
            _isSprinting = InputListener.GetKey(Sprint);
            _isJumping = InputListener.GetKey(Jump);
            _isCrouching = InputListener.GetKey(Crouch);


            //Calculate input values to reflect strafing in correlation to player direction.
            //Calculate and alter final speed values depending on whenever or not the player is using x input.
            _trueInputVector = transform.right * _inputVector.x + transform.forward * _inputVector.y;
            _totalSpeed = _isSprinting && !_isCrouching && CanStand() ? movementSpeed * sprintMultiplier :
                _isCrouching || !CanStand() ? movementSpeed * crouchMultiplier : movementSpeed;

            //Calculate and alter cameraFOV depending on player's input.
            float currentFOV = playerCamera.m_Lens.FieldOfView;
            currentFOV = _isSprinting && !_isCrouching && CanStand()
                ? Mathf.Lerp(currentFOV, _sprintFOV, 0.25f)
                : Mathf.Lerp(currentFOV, _originalFOV, 0.25f);
            playerCamera.m_Lens.FieldOfView = currentFOV;

            if (!CameraLocked)
                //Call method that handles player rotation on mouse input.
                fpcHandler.RotatePlayerHorizontally(transform, _lookValue, sensitivity);

            //Call method that alters collision's size depending on whenever or not player is crouching.
            OnCrouchAlterPlayerHeight(_isCrouching);


            ONUpdateCallback?.Invoke();

            if (InputListener.GetKeyDown(Escape))
            {
                EventManager.TriggerEvent(WeaponShop.CloseShop);
            }
        }

        /// <summary>
        /// Changes the camera's position as well as the collision heights to make the player able to crouch.
        /// </summary>
        /// <param name="isCrouching">Input check</param>
        private void OnCrouchAlterPlayerHeight(bool isCrouching)
        {
            var position = transform.position;
            if (isCrouching)
            {
                _standingHeight = position.y;
                _collisionBody.height = 1f;
                _collisionBody.center = new Vector3(0, -0.5f, 0);
                playerCamera.m_Orbits[1].m_Height = -0.5f;

                return;
            }

            if (CanStand())
            {
                _collisionBody.center = Vector3.zero;
                playerCamera.m_Orbits[1].m_Height = 0;
                _collisionBody.height = 2f;
            }
        }

        /// <summary>
        /// Checks whenever the player can stand by using an OverlapBox above the player's head.
        /// </summary>
        /// <returns>Returns true if there is no object detected above the player's head.</returns>
        private bool CanStand()
        {
            List<Collider> foundObjects = Physics
                .OverlapBox(TopPositionOfCollider, sealingCheckSize, transform.rotation, movementCheckLayer).ToList();
            return foundObjects.FindAll(c => c != this._collisionBody).Count == 0;
        }

        private void FixedUpdate()
        {
            var velocity = _physics.velocity;
            velocity = new Vector3(_trueInputVector.x * _totalSpeed, velocity.y, _trueInputVector.z * _totalSpeed);
            _physics.velocity = velocity;

            //InteractWithEntities(currencyPickupRange, pickupMask);

            if (_isJumping && IsGrounded() && !_isCrouching)
                _physics.AddForce(Vector3.up * (jumpForce * 10), ForceMode.VelocityChange);
        }


        /// <summary>
        /// Checks if there are any gameObjects bellow the player's feet.
        /// </summary>
        /// <returns>Returns true if there is at least one gameObject bellow the player's feet.</returns>
        private bool IsGrounded()
        {
            List<Collider> foundObjects = Physics.OverlapBox(BottonPositionOfCollider, groundCheckSize,
                transform.rotation, movementCheckLayer).ToList();
            return foundObjects.FindAll(c => c != _collisionBody).Count != 0;
        }


        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Gizmos.color = Color.red;
            if (IsGrounded()) Gizmos.color = Color.green;
            Gizmos.DrawCube(BottonPositionOfCollider, groundCheckSize * 2f);

            Gizmos.color = Color.red;
            if (!CanStand()) Gizmos.color = Color.green;
            Gizmos.DrawCube(TopPositionOfCollider, sealingCheckSize * 2f);
        }


        private void LateUpdate()
        {
            if (!CameraLocked)
                fpcHandler.RotateCameraVertically(playerCamera.transform, _lookValue, sensitivity);
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

        private void AddListenersToEventManager()
        {
            EventManager.AddListener<Action<bool>>(SetCursorActiveEvent, SetCameraAndCursorActive);
            EventManager.AddListener<Action<HealthComponent.CameraBehaivour>>(CameraFallBehaivourEvent,
                value => HealthComponent.SetCameraBehaivour(playerCamera, transform, value));
            EventManager.AddListener<Action>(ConstantlyLookTowardsThePlayerEvent,
                () => HealthComponent.RotateCameraTowards(playerCamera, transform));
            EventManager.AddListener<Func<Vector3, Vector3>>(MoveEntityEvent, MovePlayer);
        }


        private void RemoveListenersFromEventManager()
        {
            EventManager.RemoveListener<Action<bool>>(SetCursorActiveEvent, SetCameraAndCursorActive);
            EventManager.RemoveListener<Action<HealthComponent.CameraBehaivour>>(CameraFallBehaivourEvent,
                value => HealthComponent.SetCameraBehaivour(playerCamera, transform, value));
            EventManager.RemoveListener<Action>(ConstantlyLookTowardsThePlayerEvent,
                () => HealthComponent.RotateCameraTowards(playerCamera, transform));
            EventManager.RemoveListener<Func<Vector3, Vector3>>(MoveEntityEvent, MovePlayer);
        }

        private void SetCameraAndCursorActive(bool value)
        {
            CameraLocked = value;
            // fpcHandler.AlterCursorState(value);
            EventManager.TriggerEvent(FPCameraHandler.ChangeCursorState, value);
            EventManager.TriggerEvent(InputListener.SetPlayerLookInputActiveState, !value);
            //EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, !value);
        }

        private Vector3 MovePlayer(Vector3 velocity)
        {
            _physics.MovePosition(transform.position + velocity);
            return _physics.velocity;
        }
    }
}