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
using static Player.InputController.KeyCode;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace Player
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(DamageableEntity))]
    [RequireComponent(typeof(InputController))]
    public class PlayerController : MonoBehaviour
    {
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
        private InputController _inputController;
        private CapsuleCollider _collisionBody;

        private Vector2 _inputVector;
        private float _totalSpeed;
        private Vector3 _trueInputVector;
        private float _groundCheckDelay;

        private bool _isSprinting;
        private bool _isJumping;
        private bool _isCrouching;
        private bool _isInteracting;

        public bool IsSprinting => _isSprinting;
        public bool IsCrouching => _isCrouching;
        public Vector3 LatestRespawnPos { get; set; }

        private static CustomEvent _movePlayerEvent;

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


        public CameraController PlayerCamera { get; private set; }
        public InputController Input => _inputController ? _inputController : GetComponent<InputController>();
        public WeaponController WeaponController => _weaponController;

        private InteractionController InteractionController { get; set; }


        void OnEnable()
        {
            InteractionController = InteractionController.GetInteractionController(gameObject);
            if (InteractionController)
            {
                InteractionController.ONInteractionEnterEvent += InteractWithInteractableEntities;
            }
        }


        void OnDisable()
        {
            if (InteractionController)
            {
                InteractionController.ONInteractionEnterEvent -= InteractWithInteractableEntities;
            }
        }


        private void InteractWithInteractableEntities(RaycastHit obj)
        {
            InteractableEntity entity = obj.collider.GetComponent<InteractableEntity>();
            if (entity)
            {
                switch (entity.interactionInputType)
                {
                    case InteractionInput.Hold:
                        if (_inputController.GetKey(Interact))
                            entity.OnInteract(GetComponent<Collider>());
                        break;
                    case InteractionInput.Press:
                        if (_inputController.GetKeyDown(Interact))
                            entity.OnInteract(GetComponent<Collider>());
                        break;
                }
            }
        }


        private void Awake()
        {
            _movePlayerEvent = CustomEvent.CreateEvent<Action<Vector3>>(MovePlayer, gameObject);


            //Init
            _inputController = GetComponent<InputController>();
            _collisionBody = GetComponent<CapsuleCollider>();
            _physics = GetComponent<Rigidbody>();
            PlayerCamera = GetComponent<CameraController>();
            _weaponController = GetComponent<WeaponController>();


            CameraController.SetCursorActive(gameObject, false);
        }


        private void Update()
        {
            //Get values from Input References
            _inputVector = _inputController.GetMovementInput();
            _isSprinting = _inputController.GetKey(Sprint);
            _isJumping = _inputController.GetKey(Jump);
            _isCrouching = _inputController.GetKey(Crouch) || !CanStand();


            //Calculate input values to reflect strafing in correlation to player direction.
            //Calculate and alter final speed values depending on whenever or not the player is using x input.
            _trueInputVector = transform.right * _inputVector.x + transform.forward * _inputVector.y;
            _totalSpeed = _isSprinting && !_isCrouching && CanStand() ? movementSpeed * sprintMultiplier :
                _isCrouching ? movementSpeed * crouchMultiplier : movementSpeed;


            //Call method that alters collision's size depending on whenever or not player is crouching.
            OnCrouchAlterPlayerHeight(_isCrouching);


            ONUpdateCallback?.Invoke();

            if (_inputController.GetKeyDown(Escape))
            {
                if (!WeaponShopMenu.CloseShop(gameObject))
                    PauseMenu.TogglePause(gameObject);
            }
        }

        /// <summary>
        /// Changes the camera's position as well as the collision heights to make the player able to crouch.
        /// </summary>
        /// <param name="isCrouching">Input check</param>
        private void OnCrouchAlterPlayerHeight(bool isCrouching)
        {
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
            return foundObjects.FindAll(c => c != _collisionBody).Count == 0;
        }

        private void FixedUpdate()
        {
            //Applying input and its speed to the Rigidbody while keeping the gravity intact.
            // var velocity = _physics.velocity;
            // velocity += new Vector3(_trueInputVector.x * _totalSpeed, 0, _trueInputVector.z * _totalSpeed);
            // Vector3 clampedVelocity = Vector3.ClampMagnitude(velocity, _totalSpeed);
            // velocity = new Vector3(clampedVelocity.x, velocity.y, clampedVelocity.z);
            // if (_inputVector == Vector2.zero)
            // {
            //     velocity = new Vector3(0, velocity.y, 0);
            // }
            //
            // _physics.velocity = velocity;
            //
            //
            //Better jump logic by Boards to Bits Games (https://www.youtube.com/watch?v=7KiK0Aqtmzc)


            var currentVelocity = _physics.velocity;
            currentVelocity += new Vector3(_trueInputVector.x * _totalSpeed, 0, _trueInputVector.z * _totalSpeed);
            currentVelocity = ClampVelocity(currentVelocity);
            if (_inputVector == Vector2.zero)
                currentVelocity = ResetVelocity();

            _physics.velocity = currentVelocity;

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

        private Vector3 ClampVelocity(Vector3 currentVelocity)
        {
            Vector3 clampedVelocity = Vector3.ClampMagnitude(currentVelocity, _totalSpeed);
            return new Vector3(clampedVelocity.x, currentVelocity.y, clampedVelocity.z);
        }

        private Vector3 ResetVelocity()
        {
            if (Physics.Raycast(transform.position, Vector3.down,
                out RaycastHit hitInfo, transform.localScale.y + groundCheckSize.y))
            {
                if (hitInfo.rigidbody)
                {
                    return hitInfo.rigidbody.velocity;
                }
            }

            return new Vector3(0, _physics.velocity.y, 0);
        }


        /// <summary>
        /// Checks if there are any gameObjects bellow the player's feet.
        /// </summary>
        /// <returns>Returns true if there is at least one gameObject bellow the player's feet.</returns>
        private bool IsGrounded()
        {
            return Physics.SphereCast(new Ray(transform.position, Vector3.down), groundCheckSize.x,
                transform.localScale.y + groundCheckSize.y);
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


        private void MovePlayer(Vector3 velocity)
        {
            _physics.MovePosition(Vector3.Slerp(transform.position, transform.position + velocity, 1));
        }

        public static void Move(Collider player, Vector3 velocity)
        {
            _movePlayerEvent.OnInvokeEvent(player.gameObject, velocity);
        }


        public void OnFall()
        {
            // EventManager.TriggerEvent(PlayerCamera.CameraFallBehaivourEvent, PlayerCamera.CameraBehaivour.Look);
            // EventManager.TriggerEvent(PlayerCamera.SetCursorActiveEvent, true);
            // EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            _physics.AddForce(transform.forward * 1600f, ForceMode.Acceleration);
            StartCoroutine(Respawn(4f, LatestRespawnPos));
        }


        IEnumerator Respawn(float respawnTime, Vector3 spawnPos)
        {
            float time = 0;
            while (time < respawnTime)
            {
                yield return new WaitForEndOfFrame();
                // EventManager.TriggerEvent(PlayerCamera.ConstantlyLookTowardsThePlayerEvent);
                time += Time.deltaTime;
            }

            yield return null;
            transform.position = spawnPos;
            _physics.velocity = Vector3.zero;

            // EventManager.TriggerEvent(PlayerCamera.SetCursorActiveEvent, false);
            // EventManager.TriggerEvent(PlayerCamera.CameraFallBehaivourEvent,
            //     PlayerCamera.CameraBehaivour.Follow);
            // EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);


            yield return null;
        }
    }
}