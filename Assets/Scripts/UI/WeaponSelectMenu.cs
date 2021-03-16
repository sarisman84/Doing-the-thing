using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Interactivity.Events;
using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace UI
{
    public class WeaponSelectMenu : MonoBehaviour
    {
        private List<WeaponSlot> _weaponSlots;

        private static CustomEvent _toggleWeaponSelectMenuEvent;
        private static CustomEvent _forceCloseWeaponSelectEvent;
        private static CustomEvent _getWeaponSelectMenuEvent;
        public PlayerController playerController;


        private void Awake()
        {
            _toggleWeaponSelectMenuEvent =
                CustomEvent.CreateEvent<Action<Action<int>, List<Weapon>>>(OpenMenu, playerController.gameObject);
            _forceCloseWeaponSelectEvent = CustomEvent.CreateEvent<Action>(CloseMenu,
                playerController.gameObject);
            _getWeaponSelectMenuEvent = CustomEvent.CreateEvent<Func<bool>>(IsMenuActive,
                playerController.gameObject);


            _weaponSlots = transform.GetComponentsInChildren<WeaponSlot>().ToList();
            _weaponSlots.ApplyAction(s => s.gameObject.SetActive(false));
        }

        private bool IsMenuActive()
        {
            return IsAlreadyActive;
        }

        private void OnEnable()
        {
            // _toggleWeaponSelectMenuEvent.Subscribe<Action<Action<int>, List<Weapon>>>(OpenMenu,
            //     playerController.gameObject);
        }

        private void OnDisable()
        {
            _toggleWeaponSelectMenuEvent.RemoveEvent<Action<Action<int>, List<Weapon>>>(OpenMenu);
            _forceCloseWeaponSelectEvent.RemoveEvent<Action>(CloseMenu);
        }

        private bool IsAlreadyActive { get; set; }


        public void CloseMenu()
        {
            _weaponSlots.ApplyAction(w =>
            {
                if (IsAlreadyActive)
                {
                    w.icon.sprite = null;


                    w.gameObject.SetActive(false);
                }
            });
            gameObject.SetActive(false);
            CameraController.SetCursorActive(playerController.gameObject, false);
            CameraController.SetCameraInputActive(playerController.gameObject, true);
            IsAlreadyActive = false;
        }

        public void OpenMenu(Action<int> selectWeapon, List<Weapon> weaponLibrary)
        {
            if (IsAlreadyActive)
            {
                CloseMenu();
                return;
            }


            gameObject.SetActive(true);
            int index = 0;
            IsAlreadyActive = true;
            CameraController.SetCursorActive(playerController.gameObject, true);
            CameraController.SetCameraInputActive(playerController.gameObject, false);
            _weaponSlots.ApplyAction(w => index = ApplyBehaivour(weaponLibrary, selectWeapon, w, index));
        }

        private int ApplyBehaivour(List<Weapon> weaponLibrary, Action<int> selectWeapon, WeaponSlot w, int index)
        {
            if (index >= 0 && index < weaponLibrary.Count)
            {
                w.gameObject.SetActive(true);
                var index1 = index;
                w.slotButton.onClick.AddListener(() => selectWeapon.Invoke(index1));
                w.slotButton.onClick.AddListener(() =>
                    {
                        _weaponSlots.ApplyAction(b => b.gameObject.SetActive(false));
                        CameraController.SetCursorActive(playerController.gameObject, false);
                        CameraController.SetCameraInputActive(playerController.gameObject, true);
                        IsAlreadyActive = false;
                    }
                );

                w.SetWeaponSlotIcon(weaponLibrary[index]);
                index++;
                index = Mathf.Clamp(index, -1, weaponLibrary.Count);
                index = index == weaponLibrary.Count ? -1 : index;
            }


            return index;
        }

        public static void Open(GameObject o, Action<int> selectWeapon, List<Weapon> weaponLibrary)
        {
            if (_toggleWeaponSelectMenuEvent != null)
                _toggleWeaponSelectMenuEvent.OnInvokeEvent(o, selectWeapon, weaponLibrary);
        }

        public static void Close(GameObject player)
        {
            if (_forceCloseWeaponSelectEvent != null)
                _forceCloseWeaponSelectEvent.OnInvokeEvent(player);
        }

        public static bool IsActive(GameObject player)
        {
            if (_getWeaponSelectMenuEvent != null)
                return (bool) _getWeaponSelectMenuEvent.OnInvokeEvent(player, null);
            return false;
        }
    }
}