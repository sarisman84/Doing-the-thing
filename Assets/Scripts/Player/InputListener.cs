using System;
using Extensions;
using Extensions.InputExtension;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using Event = UnityEngine.Event;

namespace Player
{
    public class InputListener : MonoBehaviour
    {
        public InputActionAsset input;
        public const string SetAllPlayerInputActiveState = "Input_SetAllPlayerInput";
        public const string SetPlayerLookInputActiveState = "Input_SetPlayerLookInput";
        public const string SetPlayerMovementInputActiveState = "Input_SetPlayerMovementInput";

        private const string c_GetInputKeyDown = "Input_GetInputKeyDown";
        private const string c_GetInputKey = "Input_GetInput";
        private const string c_GetAxisRaw = "Input_GetAxisRaw";
        private const string c_GetMouseDelta = "Input_GetMouseDelta";

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


        private bool _GetInputKeyDown(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.Attack:
                    return _shootInput.GetButtonDown().NullcheckAndConfirm(_shootInput);
                case KeyCode.Aim:
                    return _aimInput.GetButtonDown().NullcheckAndConfirm(_aimInput);
                    ;
                case KeyCode.Jump:
                    return _jumpInput.GetButtonDown().NullcheckAndConfirm(_jumpInput);
                    ;
                case KeyCode.Crouch:
                    return _crounchInput.GetButtonDown().NullcheckAndConfirm(_crounchInput);
                    ;
                case KeyCode.Sprint:
                    return _sprintInput.GetButtonDown().NullcheckAndConfirm(_sprintInput);
                    ;
                case KeyCode.WeaponSelect:
                    return _weaponSelectInput.GetButtonDown().NullcheckAndConfirm(_weaponSelectInput);
                    ;
                case KeyCode.Escape:
                    return _escapeInput.GetButtonDown().NullcheckAndConfirm(_escapeInput);

                case KeyCode.Interact:
                    return _interactInput.GetButtonDown().NullcheckAndConfirm(_interactInput);
            }

            return false;
        }

        private bool _GetInput(KeyCode value)
        {
            switch (value)
            {
                case KeyCode.Attack:
                    return _shootInput.GetInputValue<bool>().NullcheckAndConfirm(_shootInput);
                case KeyCode.Aim:
                    return _aimInput.GetInputValue<bool>().NullcheckAndConfirm(_aimInput);
                    ;
                case KeyCode.Jump:
                    return _jumpInput.GetInputValue<bool>().NullcheckAndConfirm(_jumpInput);
                    ;
                case KeyCode.Crouch:
                    return _crounchInput.GetInputValue<bool>().NullcheckAndConfirm(_crounchInput);
                    ;
                case KeyCode.Sprint:
                    return _sprintInput.GetInputValue<bool>().NullcheckAndConfirm(_sprintInput);
                    ;
                case KeyCode.WeaponSelect:
                    return _weaponSelectInput.GetInputValue<bool>().NullcheckAndConfirm(_weaponSelectInput);
                    ;
                case KeyCode.Escape:
                    return _escapeInput.GetInputValue<bool>().NullcheckAndConfirm(_escapeInput);

                case KeyCode.Interact:
                    return _interactInput.GetInputValue<bool>().NullcheckAndConfirm(_interactInput);
            }

            return false;
        }


        private Vector2 _GetAxisRaw()
        {
            return _movementInput.GetInputValue<Vector2>();
        }


        private InputActionReference _interactInput,
            _shootInput,
            _aimInput,
            _mouseDeltaInput,
            _movementInput,
            _jumpInput,
            _crounchInput,
            _sprintInput,
            _weaponSelectInput,
            _escapeInput;

        private void Awake()
        {
            AssignReferencesFromAsset();
            SetAllInputActive(true);
            SetInputEventsToEventManager(EventManager.Event.Add);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void SetInputEventsToEventManager(EventManager.Event @event)
        {
            #region Local Methods

            object m_SetAllInputActive(object value)
            {
                m_SetAllInputActive((bool) value);
                return null;
            }

            object m_SetMouseDeltaActive(object value)
            {
                _mouseDeltaInput.SetActive((bool) value);
                return null;
            }

            object m_GetInputKeyDown(object value)
            {
                return _GetInputKeyDown((KeyCode) value);
            }

            object m_GetInput(object value)
            {
                return _GetInput((KeyCode) value);
            }

            object m_SetMovementInputActive(object value)
            {
                _movementInput.SetActive((bool) value);
                return null;
            }

            object m_GetAxisRaw(object value)
            {
                return _GetAxisRaw();
            }

            object m_GetMouseDelta(object value)
            {
                return _mouseDeltaInput.GetInputValue<Vector2>();
            }

            #endregion

            switch (@event)
            {
                case EventManager.Event.Add:
                    EventManager.AddListener(SetAllPlayerInputActiveState, m_SetAllInputActive);
                    EventManager.AddListener(SetPlayerLookInputActiveState, m_SetMouseDeltaActive);
                    EventManager.AddListener(SetPlayerMovementInputActiveState, m_SetMovementInputActive);
                    EventManager.AddListener(c_GetInputKeyDown, m_GetInputKeyDown);
                    EventManager.AddListener(c_GetAxisRaw, m_GetAxisRaw);
                    EventManager.AddListener(c_GetMouseDelta, m_GetMouseDelta);
                    EventManager.AddListener(c_GetInputKey, m_GetInput);
                    break;

                case EventManager.Event.Remove:

                    EventManager.RemoveListener(SetAllPlayerInputActiveState, m_SetAllInputActive);
                    EventManager.RemoveListener(SetPlayerLookInputActiveState, m_SetMouseDeltaActive);
                    EventManager.RemoveListener(SetPlayerMovementInputActiveState, m_SetMovementInputActive);
                    EventManager.RemoveListener(c_GetInputKeyDown, m_GetInputKeyDown);
                    EventManager.RemoveListener(c_GetAxisRaw, m_GetAxisRaw);
                    EventManager.RemoveListener(c_GetMouseDelta, m_GetMouseDelta);
                    EventManager.RemoveListener(c_GetInputKey, m_GetInput);
                    break;
            }
        }


        private void AssignReferencesFromAsset()
        {
            _interactInput = InputActionReference.Create(input.FindAction("Interact"));
            _shootInput = InputActionReference.Create(input.FindAction("Attack"));
            _aimInput = InputActionReference.Create(input.FindAction("Aim"));
            _mouseDeltaInput = InputActionReference.Create(input.FindAction("Look"));
            _movementInput = InputActionReference.Create(input.FindAction("Movement"));
            _jumpInput = InputActionReference.Create(input.FindAction("Jump"));
            _crounchInput = InputActionReference.Create(input.FindAction("Crouch"));
            _sprintInput = InputActionReference.Create(input.FindAction("Sprint"));
            _weaponSelectInput = InputActionReference.Create(input.FindAction("Weapon Select"));
            _escapeInput = InputActionReference.Create(input.FindAction("Escape"));
        }


        private void OnEnable()
        {
            SetAllInputActive(true);
        }

        private void OnDisable()
        {
            SetAllInputActive(false);
            SetInputEventsToEventManager(EventManager.Event.Remove);
        }

        private void SetAllInputActive(bool value)
        {
            InputExtension.SetActiveAll(value, _interactInput, _shootInput, _aimInput, _mouseDeltaInput, _jumpInput,
                _crounchInput, _sprintInput, _weaponSelectInput, _movementInput, _escapeInput);
        }


        public static bool GetKeyDown(KeyCode code)
        {
            return (bool) EventManager.TriggerEvent(c_GetInputKeyDown, code);
        }

        public static bool GetKey(KeyCode code)
        {
            return (bool) EventManager.TriggerEvent(c_GetInputKey, code);
        }

        public static Vector2 GetAxisRaw()
        {
            return (Vector2) EventManager.TriggerEvent(c_GetAxisRaw);
        }

        public static Vector2 GetMouseDelta()
        {
            return (Vector2) EventManager.TriggerEvent(c_GetMouseDelta);
        }
    }
}