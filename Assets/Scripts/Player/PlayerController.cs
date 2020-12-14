using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Components;
using Interactivity.Pickup;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using static Player.InputListener.KeyCode;

namespace Player
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(DamageableEntity))]
    public class PlayerController : MonoBehaviour
    {
        public const string MoveEntityEvent = "Player_MoveEntity";


        //Controller information

        #region Controller Variables

        //Movement
        [Space] public float movementSpeed;
        public float sprintMultiplier;
        public float crouchMultiplier;


        public float jumpForce;
        public float fallMultipler = 2.5f;
        public float lowJumpMultiplier = 2f;

        //Collision sizes and checks for the Ground and Stand methods.
        public Vector3 groundCheckSize = Vector3.one;
        public float groundCheckDelay = .10f;
        public Vector3 sealingCheckSize = Vector3.one;
        public LayerMask movementCheckLayer;


        //Pickup information


        //Local references to components and custom classes. 
        //Local variables that store information such as speed or input.
        private Rigidbody _physics;
        private WeaponController _weaponController;
        private CapsuleCollider _collisionBody;
        public CameraController CameraController { get; private set; }
        private Vector2 _inputVector;
        private float _totalSpeed;
        private Vector3 _trueInputVector;
        private Vector2 _lookValue;
        private float _groundCheckDelay;

        private bool _isSprinting;
        private bool _isJumping;
        private bool _isCrouching;
        private bool _isInteracting;

        public bool IsSprinting => _isSprinting;
        public bool IsCrouching => _isCrouching;
        public Vector3 LatestRespawnPos { get; set; }

        #endregion


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
                return new Vector3(position.x, position.y + (_collisionBody.bounds.size.y / 2f) - 0.25f, position.z);
            }
        }

        private void Awake()
        {
            //Assigns the player as a priority target for any enemy.
            EnemyBehaivourManager.AssignNewTarget(transform);

            //Init
            _collisionBody = GetComponent<CapsuleCollider>();
            _physics = GetComponent<Rigidbody>();
            CameraController = GetComponent<CameraController>();


            _weaponController = GetComponent<WeaponController>();
        }


        private void Update()
        {
            //Get values from Input References
            _inputVector = InputListener.GetAxisRaw();
            _lookValue = InputListener.GetMouseDelta();
            _isSprinting = InputListener.GetKey(Sprint);
            _isJumping = InputListener.GetKey(Jump);
            _isCrouching = InputListener.GetKey(Crouch) || !CanStand();


            //Calculate input values to reflect strafing in correlation to player direction.
            //Calculate and alter final speed values depending on whenever or not the player is using x input.
            _trueInputVector = transform.right * _inputVector.x + transform.forward * _inputVector.y;
            _totalSpeed = _isSprinting && !_isCrouching && CanStand() ? movementSpeed * sprintMultiplier :
                _isCrouching ? movementSpeed * crouchMultiplier : movementSpeed;

            //Calculate and alter cameraFOV depending on player's input.
            // float currentFOV = CameraController.playerCamera.m_Lens.FieldOfView;
            // currentFOV = _isSprinting && !_isCrouching && CanStand()
            //     ? Mathf.Lerp(currentFOV, _sprintFOV, 0.25f)
            //     : Mathf.Lerp(currentFOV, _originalFOV, 0.25f);
            // CameraController.playerCamera.m_Lens.FieldOfView = currentFOV;


            //Call method that alters collision's size depending on whenever or not player is crouching.
            OnCrouchAlterPlayerHeight(_isCrouching);


            ONUpdateCallback?.Invoke();

            if (InputListener.GetKeyDown(Escape))
            {
                if (WeaponShop.isShopOpen && _weaponController)
                {
                    if (_weaponController.closeShopEvent)
                        _weaponController.closeShopEvent.OnInvokeEvent();
                    return;
                }

                PauseMenu.TogglePause();
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
                _collisionBody.height = 1f;
                _collisionBody.center = new Vector3(0, -0.5f, 0);

                return;
            }


            _collisionBody.center = Vector3.zero;
            _collisionBody.height = 2f;
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
            //Applying input and its speed to the Rigidbody while keeping the gravity intact.
            var velocity = _physics.velocity;
            velocity = new Vector3(_trueInputVector.x * _totalSpeed, velocity.y, _trueInputVector.z * _totalSpeed);
            _physics.velocity = velocity;


            //Better jump logic by Boards to Bits Games (https://www.youtube.com/watch?v=7KiK0Aqtmzc)
            if (_physics.velocity.y < 0)
            {
                _physics.velocity += Vector3.up * (Physics.gravity.y * (fallMultipler - 1) * Time.fixedDeltaTime);
            }
            else if (_physics.velocity.y > 0 && !_isJumping)
            {
                _physics.velocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime);
            }


            if (_isJumping && IsGrounded() && !_isCrouching)
                _physics.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }


        /// <summary>
        /// Checks if there are any gameObjects bellow the player's feet.
        /// </summary>
        /// <returns>Returns true if there is at least one gameObject bellow the player's feet.</returns>
        private bool IsGrounded()
        {
            _groundCheckDelay = _groundCheckDelay.Equals(groundCheckDelay) ? 0 : _groundCheckDelay;
            _groundCheckDelay += Time.deltaTime;
            _groundCheckDelay = Mathf.Clamp(_groundCheckDelay, 0, groundCheckDelay);
            List<Collider> foundObjects = Physics.OverlapBox(BottonPositionOfCollider, groundCheckSize,
                transform.rotation, movementCheckLayer).ToList();
            bool result = foundObjects.FindAll(c => c != _collisionBody).Count != 0;

            return result && _groundCheckDelay.Equals(groundCheckDelay);
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
            EventManager.AddListener<Func<Vector3, Vector3>>(MoveEntityEvent, MovePlayer);
        }


        private void RemoveListenersFromEventManager()
        {
            EventManager.RemoveListener<Func<Vector3, Vector3>>(MoveEntityEvent, MovePlayer);
        }


        private Vector3 MovePlayer(Vector3 velocity)
        {
            _physics.MovePosition(transform.position + velocity);
            return _physics.velocity;
        }


        public void OnFall()
        {
            EventManager.TriggerEvent(CameraController.CameraFallBehaivourEvent, CameraController.CameraBehaivour.Look);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, true);
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            _physics.AddForce(transform.forward * 1600f, ForceMode.Acceleration);
            StartCoroutine(Respawn(4f, LatestRespawnPos));
        }


        IEnumerator Respawn(float respawnTime, Vector3 spawnPos)
        {
            float time = 0;
            while (time < respawnTime)
            {
                yield return new WaitForEndOfFrame();
                EventManager.TriggerEvent(CameraController.ConstantlyLookTowardsThePlayerEvent);
                time += Time.deltaTime;
            }

            yield return null;
            transform.position = spawnPos;
            _physics.velocity = Vector3.zero;

            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, false);
            EventManager.TriggerEvent(CameraController.CameraFallBehaivourEvent,
                CameraController.CameraBehaivour.Follow);
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);


            yield return null;
        }
    }
}