using System;
using Extensions;
using Extensions.InputExtension;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using Utility.Attributes;

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

        [Expose] public InputActionAsset controlSetup;

        private InputActionReference _movementInput,
            _lookInput,
            _jumpInput,
            _crouchInput,
            _sprintInput,
            _fireInput,
            _weaponSelectInput,
            _interactInput,
            _aimInput,
            _escapeInput;

        private bool _useMovement = true;

        private static CustomEvent _onMovementToggle;

        private void Awake()
        {
            FetchReferencesFromAsset();
        }

        private void OnEnable()
        {
            _onMovementToggle = CustomEvent.CreateEvent<Action<bool>>(ToggleMovement, gameObject);
            SetInputReferenceActive(true, _movementInput, _lookInput, _jumpInput, _crouchInput, _sprintInput,
                _fireInput, _weaponSelectInput, _interactInput, _aimInput, _escapeInput);
        }


        private void OnDisable()
        {
            _onMovementToggle.RemoveEvent<Action<bool>>(ToggleMovement);
            SetInputReferenceActive(false, _movementInput, _lookInput, _jumpInput, _crouchInput, _sprintInput,
                _fireInput, _weaponSelectInput, _interactInput, _aimInput, _escapeInput);
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
            _interactInput = InputActionReference.Create(controlSetup.FindAction("Interact"));
            _fireInput = InputActionReference.Create(controlSetup.FindAction("Attack"));
            _aimInput = InputActionReference.Create(controlSetup.FindAction("Aim"));
            _lookInput = InputActionReference.Create(controlSetup.FindAction("Look"));
            _movementInput = InputActionReference.Create(controlSetup.FindAction("Movement"));
            _jumpInput = InputActionReference.Create(controlSetup.FindAction("Jump"));
            _crouchInput = InputActionReference.Create(controlSetup.FindAction("Crouch"));
            _sprintInput = InputActionReference.Create(controlSetup.FindAction("Sprint"));
            _weaponSelectInput = InputActionReference.Create(controlSetup.FindAction("Weapon Select"));
            _escapeInput = InputActionReference.Create(controlSetup.FindAction("Escape"));
        }


        public bool GetKeyDown(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Attack:
                    return _fireInput.GetButtonDown();
                case KeyCode.Aim:
                    return _aimInput.GetButtonDown();
                case KeyCode.Jump:
                    return _jumpInput.GetButtonDown();
                case KeyCode.Crouch:
                    return _crouchInput.GetButtonDown();
                case KeyCode.Sprint:
                    return _sprintInput.GetButtonDown();
                case KeyCode.WeaponSelect:
                    return _weaponSelectInput.GetButtonDown();
                case KeyCode.Escape:
                    return _escapeInput.GetButtonDown();
                case KeyCode.Interact:
                    return _interactInput.GetButtonDown();
            }

            return false;
        }

        public bool GetKey(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Attack:
                    return _fireInput.GetInputValue<bool>();
                case KeyCode.Aim:
                    return _aimInput.GetInputValue<bool>();
                case KeyCode.Jump:
                    return _jumpInput.GetInputValue<bool>();
                case KeyCode.Crouch:
                    return _crouchInput.GetInputValue<bool>();
                case KeyCode.Sprint:
                    return _sprintInput.GetInputValue<bool>();
                case KeyCode.WeaponSelect:
                    return _weaponSelectInput.GetInputValue<bool>();
                case KeyCode.Escape:
                    return _escapeInput.GetInputValue<bool>();
                case KeyCode.Interact:
                    return _interactInput.GetInputValue<bool>();
            }

            return false;
        }

        public Vector2 GetMovementInput()
        {
            Vector2 input = _movementInput.action.ReadValue<Vector2>();

            return _useMovement ? input : Vector2.zero;
        }

        public Vector2 GetMouseDelta()
        {
            Vector2 input = _lookInput.action.ReadValue<Vector2>();
            return input;
        }

        private void ToggleMovement(bool value)
        {
            _useMovement = value;
        }

        public static void SetMovementInputActive(GameObject targetPlayer, bool b)
        {
            if (_onMovementToggle)
                _onMovementToggle.OnInvokeEvent(targetPlayer, b);
        }
    }
}