using System;
using System.Collections.Generic;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonController : MonoBehaviour
    {
        //TODO
        /*
         * Implement Input [Done]
         * Implement Movement (Sprinting [Done], Jumping [Done], Crouching [Done], Sliding, Climbing)
         * Implement Camera Rotation [Done]
         * Implement Animations
         * Implement Sounds
         */

        #region Input Referencing (new input system)

        public InputActionAsset inputMap;

        InputAction m_HorizontalInputReference;
        InputAction m_JumpButtonReference;
        InputAction m_CrouchButtonReference;
        InputAction m_SprintButtonReference;


        public Vector3 HorizontalInput
        {
            get
            {
                Vector2 input = m_HorizontalInputReference.ReadValue<Vector2>();
                if (CanSlide)
                    return transform.right * (input.x * sidewayMovementMultiplierOnSlide);
                return transform.right * input.x + transform.forward * input.y;
            }
        }

        public bool JumpInput => m_JumpButtonReference.ReadValue<float>() > 0 && !CrouchInput && IsGrounded();
        public bool SprintInput => m_SprintButtonReference.ReadValue<float>() > 0;

        public bool CrouchInput =>
            (m_CrouchButtonReference.ReadValue<float>() > 0 && !m_IsInTheAir) || IsThereAnObjectAbove();

        private InputManager m_Manager;


        #region Calculation Shortcuts

        public bool CanSlide => m_PhysicsComponent.velocity.magnitude > (movementSpeed * 100) && !m_IsSliding &&
                                CrouchInput;

        public float PlayerHeight =>
            m_PlayerCollider ? m_PlayerCollider.height : GetComponent<Collider>().bounds.size.y;

        public Vector3 BottomPosition
        {
            get
            {
                var position = transform.position;
                Vector3 pos = new Vector3(position.x, position.y - PlayerHeight / (CrouchInput ? 1f : 2f) + 0.15f,
                    position.z);
                return pos;
            }
        }

        #endregion

        #endregion

        [Header("Settings")] public float movementSpeed;
        public float sprintMultiplier;
        public float crouchMultiplier;
        public float slideMultiplier;
        public float slideStopThreshold = 0.2f;
        public float sidewayMovementMultiplierOnSlide;
        [Header("Jump Settings")] public float jumpForce;
        public float skinWidth;
        public LayerMask groundCollisionMask;

        #region Component fetching

        private Rigidbody m_PhysicsComponent;
        private Camera m_PlayerCam;
        private CapsuleCollider m_PlayerCollider;
        private Transform m_CameraAnchor;
        private Animator m_PlayerAnimator;

        #endregion

        #region Private Variables

        private float m_JumpModifier;
        private bool m_IsInTheAir;
        private bool m_IsSliding;
        private float m_InitialCollisionHeight;
        private float m_trueSpeed;
        private float m_rigidBodyVelocity;
        private Vector3 m_InititalCollisionCenter;
        private Vector3 m_InitialCameraAnchorPos;
        private float m_RegisteredMovementSpeed;

        #endregion

        private void OnEnable()
        {
            (m_Manager ??= new InputManager(inputMap)
                .GetAction("Movement", out m_HorizontalInputReference)
                .GetAction("Jump", out m_JumpButtonReference)
                .GetAction("Crouch", out m_CrouchButtonReference)
                .GetAction("Sprint", out m_SprintButtonReference)).EnableInput();
        }

        private void OnDisable()
        {
            m_Manager.DisableInput();
        }

        private void Awake()
        {
            m_PhysicsComponent = GetComponent<Rigidbody>();
            m_JumpModifier = jumpForce;
            m_PlayerCam = Camera.main;
            m_PlayerCollider = GetComponent<CapsuleCollider>();
            SetCursorState(false);

            m_CameraAnchor = transform.GetChildWithTag("Player/CameraAnchor");
            m_PlayerAnimator = transform.GetComponentInChildren<Animator>();

            m_InitialCollisionHeight = m_PlayerCollider.height;
            m_InititalCollisionCenter = m_PlayerCollider.center;
            m_InitialCameraAnchorPos = m_CameraAnchor.localPosition;
        }


        private void Update()
        {
            //Camera to Player Rotation 
            transform.rotation = new Quaternion(0, m_PlayerCam.transform.rotation.y, 0,
                m_PlayerCam.transform.rotation.w);

            UpdatePlayerCollision();
            
            m_PlayerAnimator.SetFloat("Velocity",m_PhysicsComponent.velocity.magnitude);
        }

        private void UpdatePlayerCollision()
        {
            bool isPlayerHeadClear = !IsThereAnObjectAbove();
            if (CrouchInput)
            {
                m_PlayerCollider.height = m_InitialCollisionHeight / 2f;
                m_PlayerCollider.center = m_InititalCollisionCenter - Vector3.up * (m_InitialCollisionHeight / 4f);
                m_CameraAnchor.localPosition = m_InitialCameraAnchorPos - Vector3.up * (m_InitialCollisionHeight / 4f);
            }
            else if (Math.Abs(m_PlayerCollider.height - m_InitialCollisionHeight) > 0.001f &&
                     m_PlayerCollider.center != m_InititalCollisionCenter &&
                     m_CameraAnchor.position != m_InititalCollisionCenter && isPlayerHeadClear)
            {
                m_PlayerCollider.height = m_InitialCollisionHeight;
                m_PlayerCollider.center = m_InititalCollisionCenter;
                m_CameraAnchor.localPosition = m_InitialCameraAnchorPos;
            }
        }


        private void FixedUpdate()
        {
            //Physics Calculations and appliances
            m_trueSpeed = CalculateAcceleration();


            // if (CanSlide)
            // {
            //     m_PhysicsComponent.AddForce(transform.forward * (movementSpeed * slideMultiplier),
            //         ForceMode.VelocityChange);
            //     m_IsSliding = true;
            // }
            // else

            if (!m_IsSliding && !m_IsInTheAir)
            {
                m_PhysicsComponent.AddForce(HorizontalInput * (m_trueSpeed * Time.fixedDeltaTime),
                    ForceMode.VelocityChange);
                m_PhysicsComponent.velocity =
                    Vector3.ClampMagnitude(new Vector3(m_PhysicsComponent.velocity.x, 0, m_PhysicsComponent.velocity.z),
                        CalculateMaxMovementSpeed()) +
                    Vector3.ClampMagnitude(new Vector3(0, m_PhysicsComponent.velocity.y, 0), 30);

                //If no input is being detected, reset the velocity
                if (HorizontalInput.magnitude <= 0.01f)
                {
                    m_PhysicsComponent.velocity = Vector3.Lerp(m_PhysicsComponent.velocity,
                        new Vector3(0, m_PhysicsComponent.velocity.y, 0), 0.00000001f);
                }
            }

            m_IsSliding = CalculateAndApplySliding();

            // if (m_PhysicsComponent.velocity.magnitude <= slideStopThreshold || !CrouchInput)
            // {
            //     m_IsSliding = false;
            // }


            ApplyExternalForces();
            CheckAndUpdatePlayerJumpState();
            if (JumpInput && !m_IsInTheAir)
            {
                m_PhysicsComponent.AddForce(Vector3.up * m_JumpModifier, ForceMode.Impulse);
                m_IsInTheAir = true;
            }
        }

        private float counter;

        private bool CalculateAndApplySliding()
        {
            m_rigidBodyVelocity = m_PhysicsComponent.velocity.magnitude;
            if (m_PhysicsComponent.velocity.magnitude > movementSpeed && !m_IsSliding)
            {
                m_RegisteredMovementSpeed = movementSpeed;
                counter = 0;
            }
            else
            {
                counter += Time.deltaTime;
                if (counter >= 1f)
                {
                    m_RegisteredMovementSpeed = 0;
                }
            }

            //If the registedMovementSpeed is higher than the normal movementSpeed for the past 1 second and we are not already sliding, slide.
            if (m_RegisteredMovementSpeed != 0 && !m_IsSliding && CrouchInput)
            {
                m_PhysicsComponent.AddForce(transform.forward * (m_rigidBodyVelocity * 2f),
                    ForceMode.VelocityChange);
                return true;
            }

            if ((m_PhysicsComponent.velocity.magnitude <= slideStopThreshold || !CrouchInput) && m_IsSliding)
                return false;

            return false;
        }

        private float CalculateAcceleration()
        {
            if (SprintInput)
                return movementSpeed * 100 * sprintMultiplier;
            if (CrouchInput)
                return movementSpeed * 100 * crouchMultiplier;

            return movementSpeed * 100;
        }

        public float CalculateMaxMovementSpeed()
        {
            if (SprintInput)
                return movementSpeed * sprintMultiplier;
            if (CrouchInput)
                return movementSpeed * crouchMultiplier;
            return movementSpeed;
        }

        #region Physics Stuff

        private void ApplyExternalForces()
        {
            GroundRaycast(out var hit);
            if (hit.collider && hit.rigidbody)
            {
                m_PhysicsComponent.velocity = hit.rigidbody.velocity;
            }
        }

        private void CheckAndUpdatePlayerJumpState()
        {
            if (IsGrounded() && m_IsInTheAir && m_PhysicsComponent.velocity.normalized.y <= 0)
                m_IsInTheAir = false;
        }


        private bool GroundRaycast(out RaycastHit hit)
        {
            return Physics.Raycast(BottomPosition,
                -Vector3.up, out hit, skinWidth, groundCollisionMask);
        }


        private bool IsGrounded()
        {
            bool result = GroundRaycast(out _);
            Debug.DrawRay(BottomPosition, -Vector3.up * (skinWidth), result ? Color.green : Color.red);
            return result;
        }

        private bool IsThereAnObjectAbove()
        {
            var position = transform.position;
            bool result = Physics.Raycast(position, Vector3.up, PlayerHeight * skinWidth, groundCollisionMask);
            Debug.DrawRay(position, Vector3.up * (PlayerHeight * skinWidth), result ? Color.green : Color.red);
            return result;
        }

        #endregion

        #region Camera Stuff

        public void SetCursorState(bool value)
        {
            Cursor.visible = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }

        #endregion
    }

    class InputManager
    {
        private readonly List<InputAction> m_RegistedActions;

        public InputManager(InputActionAsset asset)
        {
            try
            {
                m_RegistedActions = new List<InputAction>();
                m_RegistedActions.AddRange(asset.actionMaps[0].actions);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public InputManager GetAction(string name, out InputAction result)
        {
            result = m_RegistedActions.Find(a => a.name.Contains(name));
            return this;
        }

        public void EnableInput()
        {
            foreach (var registeredReference in m_RegistedActions)
            {
                registeredReference.Enable();
            }
        }

        public void DisableInput()
        {
            foreach (var registeredReference in m_RegistedActions)
            {
                registeredReference.Disable();
            }
        }
    }
}