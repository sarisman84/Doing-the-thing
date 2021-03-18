using System;
using Extensions;
using Extensions.InputExtension;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        public enum KeyCode
        {
            Attack,
            Aim,
            Jump,
            Crouch,
            Sprint,
            WeaponSelect,
            Escape,
            Interact
        }

        public enum Axis
        {
            Horizontal,
            Vertical
        }

        public InputActionAsset controlSetup;

        [SerializeField] private InputActionReference movementInput,
            lookInput,
            jumpInput,
            crouchInput,
            sprintInput,
            fireInput,
            weaponSelectInput,
            interactInput,
            aimInput,
            escapeInput;

        private bool _useMovement = true;

        private static CustomEvent onMovementToggle;
        private void Awake()
        {
            FetchReferencesFromAsset();
        }

        private void OnEnable()
        {
            onMovementToggle = CustomEvent.CreateEvent<Action<bool>>(ToggleMovement, gameObject);
            SetInputReferenceActive(true, movementInput, lookInput, jumpInput, crouchInput, sprintInput,
                fireInput, weaponSelectInput, interactInput, aimInput, escapeInput);
        }

        

        private void OnDisable()
        {
            onMovementToggle.RemoveEvent<Action<bool>>(ToggleMovement);
            SetInputReferenceActive(false, movementInput, lookInput, jumpInput, crouchInput, sprintInput,
                fireInput, weaponSelectInput, interactInput, aimInput, escapeInput);
        }


        private void SetInputReferenceActive(bool value, params InputActionReference[] references)
        {
            foreach (var reference in references)
            {
                if (!reference) continue;

                if (value)
                    reference.action.Enable();
                else
                    reference.action.Disable();
            }
        }


        private void FetchReferencesFromAsset()
        {
            interactInput = InputActionReference.Create(controlSetup.FindAction("Interact"));
            fireInput = InputActionReference.Create(controlSetup.FindAction("Attack"));
            aimInput = InputActionReference.Create(controlSetup.FindAction("Aim"));
            lookInput = InputActionReference.Create(controlSetup.FindAction("Look"));
            movementInput = InputActionReference.Create(controlSetup.FindAction("Movement"));
            jumpInput = InputActionReference.Create(controlSetup.FindAction("Jump"));
            crouchInput = InputActionReference.Create(controlSetup.FindAction("Crouch"));
            sprintInput = InputActionReference.Create(controlSetup.FindAction("Sprint"));
            weaponSelectInput = InputActionReference.Create(controlSetup.FindAction("Weapon Select"));
            escapeInput = InputActionReference.Create(controlSetup.FindAction("Escape"));
        }


        public bool GetKeyDown(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Attack:
                    return fireInput.GetButtonDown();
                case KeyCode.Aim:
                    return aimInput.GetButtonDown();
                case KeyCode.Jump:
                    return jumpInput.GetButtonDown();
                case KeyCode.Crouch:
                    return crouchInput.GetButtonDown();
                case KeyCode.Sprint:
                    return sprintInput.GetButtonDown();
                case KeyCode.WeaponSelect:
                    return weaponSelectInput.GetButtonDown();
                case KeyCode.Escape:
                    return escapeInput.GetButtonDown();
                case KeyCode.Interact:
                    return interactInput.GetButtonDown();
            }

            return false;
        }

        public bool GetKey(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Attack:
                    return fireInput.GetInputValue<bool>();
                case KeyCode.Aim:
                    return aimInput.GetInputValue<bool>();
                case KeyCode.Jump:
                    return jumpInput.GetInputValue<bool>();
                case KeyCode.Crouch:
                    return crouchInput.GetInputValue<bool>();
                case KeyCode.Sprint:
                    return sprintInput.GetInputValue<bool>();
                case KeyCode.WeaponSelect:
                    return weaponSelectInput.GetInputValue<bool>();
                case KeyCode.Escape:
                    return escapeInput.GetInputValue<bool>();
                case KeyCode.Interact:
                    return interactInput.GetInputValue<bool>();
            }

            return false;
        }

        public Vector2 GetMovementInput()
        {
            Vector2 input = movementInput.action.ReadValue<Vector2>();
            
            return _useMovement ? input : Vector2.zero;
        }

        public Vector2 GetMouseDelta()
        {
            Vector2 input = lookInput.action.ReadValue<Vector2>();
            return input;
        }
        
        private void ToggleMovement(bool value)
        {
            _useMovement = value;
        }

        public static void SetMovementInputActive(GameObject targetPlayer, bool b)
        {
            if (onMovementToggle)
                onMovementToggle.OnInvokeEvent(targetPlayer, b);
        }
    }
}