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
         * Implement Input
         * Implement Movement
         * Implement Camera Rotation
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
                return transform.right * input.x + transform.forward * input.y;
            }
        }

        public bool JumpInput => m_JumpButtonReference.ReadValue<float>() > 0 && !CrouchInput;
        public bool SprintInput => m_SprintButtonReference.ReadValue<float>() > 0 && !CrouchInput;
        public bool CrouchInput => m_CrouchButtonReference.ReadValue<float>() > 0 && !m_IsInTheAir;

        private InputManager m_Manager;

        #endregion

        [Header("Settings")] public float movementSpeed;
        public float sprintMultiplier;
        public float crouchMultiplier;
        [Header("Jump Settings")] public float jumpForce;
        public float skinWidth;
        public LayerMask groundCollisionMask;

        #region Component fetching

        private Rigidbody m_PhysicsComponent;
        private Camera m_PlayerCam;
        private CapsuleCollider m_PlayerCollider;
        private Transform m_CameraAnchor;

        #endregion

        private float m_JumpModifier;
        private bool m_IsInTheAir;
        private float m_InitialCollisionHeight;
        private Vector3 m_InititalCollisionCenter;
        private Vector3 m_InitialCameraAnchorPos;

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
            
            
            
        }

        private void UpdatePlayerCollision()
        {
           
            if (CrouchInput)
            {
               
                m_PlayerCollider.height = m_InitialCollisionHeight / 2f;
                m_PlayerCollider.center = m_InititalCollisionCenter - Vector3.up * (m_InitialCollisionHeight / 4f);
                m_CameraAnchor.localPosition = m_InitialCameraAnchorPos - Vector3.up * (m_InitialCollisionHeight / 4f);
            }
            else if(m_PlayerCollider.height != m_InitialCollisionHeight && m_PlayerCollider.center != m_InititalCollisionCenter && m_CameraAnchor.position != m_InititalCollisionCenter)
            {
                m_PlayerCollider.height = m_InitialCollisionHeight;
                m_PlayerCollider.center = m_InititalCollisionCenter;
                m_CameraAnchor.localPosition = m_InitialCameraAnchorPos;
            }
            
            
        }


        private void FixedUpdate()
        {
            //Physics Calculations and appliances
            m_PhysicsComponent.MovePosition(m_PhysicsComponent.position + HorizontalInput *
                ((SprintInput ? movementSpeed * sprintMultiplier : CrouchInput ? crouchMultiplier * movementSpeed : movementSpeed) * Time.fixedDeltaTime));
            ApplyExternalForces();
            CheckAndUpdatePlayerJumpState();
            if (JumpInput && IsGrounded() && !m_IsInTheAir)
            {
                m_PhysicsComponent.AddForce(Vector3.up * m_JumpModifier, ForceMode.Impulse);
                m_IsInTheAir = true;
            }
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

        #endregion

        #region Camera Stuff

        public void SetCursorState(bool value)
        {
            Cursor.visible = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }
        #endregion

        public Vector3 BottomPosition
        {
            get
            {
                Vector3 pos = new Vector3(transform.position.x,
                    transform.position.y - (GetComponent<Collider>().bounds.size.y / 2f) + 0.15f,
                    transform.position.z);
                return pos;
            }
        }
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