using System;
using Extensions;
using Extensions.InputExtension;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class InputListener
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameLoad()
        {
            InputListener listener = new InputListener();
            listener._input = Resources.Load<InputActionAsset>("Controls");
            if (listener._input.Equals(null)) return;
            listener.AssignReferencesFromAsset();
            listener.SetAllInputActive(true);
            listener.SetInputEventsToEventManager(EventManager.ManagerAction.Add);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        private InputActionAsset _input;
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


        private void SetInputEventsToEventManager(EventManager.ManagerAction managerAction)
        {
            switch (managerAction)
            {
                case EventManager.ManagerAction.Add:
                    EventManager.AddListener<Action<bool>>(SetAllPlayerInputActiveState, SetAllInputActive);
                    EventManager.AddListener<Action<bool>>(SetPlayerLookInputActiveState,
                        value => _mouseDeltaInput.SetActive(value));
                    EventManager.AddListener<Action<bool>>(SetPlayerMovementInputActiveState,
                        value => _movementInput.SetActive(value));
                    EventManager.AddListener<Func<KeyCode, bool>>(c_GetInputKey, _GetInput);
                    EventManager.AddListener<Func<Vector2>>(c_GetAxisRaw, _GetAxisRaw);
                    EventManager.AddListener<Func<Vector2>>(c_GetMouseDelta,
                        () => _mouseDeltaInput.GetInputValue<Vector2>());
                    EventManager.AddListener<Func<KeyCode, bool>>(c_GetInputKeyDown, _GetInputKeyDown);
                    break;

                case EventManager.ManagerAction.Remove:

                    EventManager.RemoveListener<Action<bool>>(SetAllPlayerInputActiveState, SetAllInputActive);
                    EventManager.RemoveListener<Action<bool>>(SetPlayerLookInputActiveState,
                        value => _mouseDeltaInput.SetActive(value));
                    EventManager.RemoveListener<Action<bool>>(SetPlayerMovementInputActiveState,
                        value => _movementInput.SetActive(value));
                    EventManager.RemoveListener<Func<KeyCode, bool>>(c_GetInputKey, _GetInput);
                    EventManager.RemoveListener<Func<Vector2>>(c_GetAxisRaw, _GetAxisRaw);
                    EventManager.RemoveListener<Func<Vector2>>(c_GetMouseDelta,
                        () => _mouseDeltaInput.GetInputValue<Vector2>());
                    EventManager.RemoveListener<Func<KeyCode, bool>>(c_GetInputKeyDown, _GetInputKeyDown);
                    break;
            }
        }


        private void AssignReferencesFromAsset()
        {
            _interactInput = InputActionReference.Create(_input.FindAction("Interact"));
            _shootInput = InputActionReference.Create(_input.FindAction("Attack"));
            _aimInput = InputActionReference.Create(_input.FindAction("Aim"));
            _mouseDeltaInput = InputActionReference.Create(_input.FindAction("Look"));
            _movementInput = InputActionReference.Create(_input.FindAction("Movement"));
            _jumpInput = InputActionReference.Create(_input.FindAction("Jump"));
            _crounchInput = InputActionReference.Create(_input.FindAction("Crouch"));
            _sprintInput = InputActionReference.Create(_input.FindAction("Sprint"));
            _weaponSelectInput = InputActionReference.Create(_input.FindAction("Weapon Select"));
            _escapeInput = InputActionReference.Create(_input.FindAction("Escape"));
        }


        private void OnEnable()
        {
            SetAllInputActive(true);
        }

        private void OnDisable()
        {
            SetAllInputActive(false);
            SetInputEventsToEventManager(EventManager.ManagerAction.Remove);
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
            return (bool)EventManager.TriggerEvent(c_GetInputKey, code);
           
        }

        public static Vector2 GetAxisRaw()
        {
            return (Vector2)EventManager.TriggerEvent(c_GetAxisRaw);
         
        }

        public static Vector2 GetMouseDelta()
        {
            return (Vector2)EventManager.TriggerEvent(c_GetMouseDelta);
          
            
        }
    }
}