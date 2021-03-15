using System;
using Extensions;
using Extensions.InputExtension;
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

        public InputActionAsset controlSetup;

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

        private void Awake()
        {
            FetchReferencesFromAsset();
        }

        private void OnEnable()
        {
            SetInputReferenceActive(true, _movementInput, _lookInput, _jumpInput, _crouchInput, _sprintInput,
                _fireInput, _weaponSelectInput, _interactInput, _aimInput, _escapeInput);
        }

        private void OnDisable()
        {
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


        private bool TriggerButton(KeyCode keyCode)
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

        private bool HoldButton(KeyCode keyCode)
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
    }
}