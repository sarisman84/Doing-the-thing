using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class FirstPersonController : MonoBehaviour
    {
        public InputActionReference movementInput, jumpInput, lookInput, crouchInput, sprintInput;
        public InputActionReference aimSightsInput, primaryFireInput, secondaryFireInput;

        [Space] public float movementSpeed;
        public float sprintMultiplier;
        public float crouchMultiplier;
        [Range(50, 300f)] public float sensitivity = 200f;
        public float jumpForce;
        public Vector3 groundCheckSize = Vector3.one;
        public Vector3 sealingCheckSize = Vector3.one;

        public CinemachineFreeLook playerCamera;
        [Range(10f, 100f)] public float fieldOfView = 60f;
        [Range(10f, 100f)] public float fieldOfViewWhileSprinting = 90f;


        private Rigidbody _physics;
        private CapsuleCollider _collisionBody;
        private FPCameraHandler _fpcHandler;
        private WeaponController _weaponController;
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


        public event Action onUpdateCallback;


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
            _collisionBody = GetComponent<CapsuleCollider>();
            _physics = GetComponent<Rigidbody>();
            _fpcHandler = new FPCameraHandler();

            _originalFOV = fieldOfView;
            _sprintFOV = fieldOfViewWhileSprinting;

            playerCamera.m_Lens.FieldOfView = _originalFOV;
            InputExtension.SetActiveAll(false, movementInput, sprintInput, jumpInput, crouchInput, lookInput, aimSightsInput, primaryFireInput, secondaryFireInput);
            _weaponController = new WeaponController(aimSightsInput, primaryFireInput, secondaryFireInput, this);
        }


        private void Update()
        {
            //Get values from Input References
            _inputVector = movementInput.GetInputValue<Vector2>();
            _lookValue = lookInput.GetInputValue<Vector2>();
            _isSprinting = sprintInput.GetInputValue<bool>();
            _isJumping = jumpInput.GetInputValue<bool>();
            _isCrouching = crouchInput.GetInputValue<bool>();

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

            //Call method that handles player rotation on mouse input.
            _fpcHandler.RotatePlayerHorizontally(transform, _lookValue, sensitivity);
            
            //Call method that alters collision's size depending on whenever or not player is crouching.
            OnCrouchAlterPlayerHeight(_isCrouching);

            onUpdateCallback?.Invoke();
        }

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

        private bool CanStand()
        {
            List<Collider> foundObjects = Physics.OverlapBox(TopPositionOfCollider, sealingCheckSize).ToList();
            return foundObjects.FindAll(c => c != this._collisionBody).Count == 0;
        }

        private void FixedUpdate()
        {
            var velocity = _physics.velocity;
            velocity = new Vector3(_trueInputVector.x * _totalSpeed, velocity.y, _trueInputVector.z * _totalSpeed);
            _physics.velocity = velocity;

            if (_isJumping && IsGrounded() && !_isCrouching)
                _physics.AddForce(Vector3.up * (jumpForce * 10), ForceMode.VelocityChange);
        }

        private bool IsGrounded()
        {
            List<Collider> foundObjects = Physics.OverlapBox(BottonPositionOfCollider, groundCheckSize).ToList();
            return foundObjects.FindAll(c => c != this._collisionBody).Count != 0;
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
            _fpcHandler.RotateCameraVertically(playerCamera.transform, _lookValue, sensitivity);
        }

     

        


        

        private void OnDisable()
        {
            
            InputExtension.SetActiveAll(false, movementInput, sprintInput, jumpInput, crouchInput, lookInput, aimSightsInput, primaryFireInput, secondaryFireInput);
        }

        private void OnEnable()
        {
            InputExtension.SetActiveAll(true, movementInput, sprintInput, jumpInput, crouchInput, lookInput, aimSightsInput, primaryFireInput, secondaryFireInput);
        }
    }
}