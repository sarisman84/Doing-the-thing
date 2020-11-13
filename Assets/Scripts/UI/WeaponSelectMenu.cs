﻿using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player;
using Player.Weapons;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;

namespace UI
{
    public class WeaponSelectMenu
    {
        private const string OpenWeaponSelection = "UI_WeaponSelectMenu";
        private readonly List<WeaponSlot> _weaponSlots;
        private Canvas _canvas;

        public WeaponSelectMenu(Canvas asset)
        {
            _canvas = asset;
            Transform selectCanvas = _canvas.transform;
            _weaponSlots = selectCanvas.GetComponentsInChildren<WeaponSlot>().ToList();

            EventManager.AddListener<Action<Action<int>, List<Weapon>>>(OpenWeaponSelection, OpenMenu);
            _weaponSlots.ApplyAction(s => s.gameObject.SetActive(false));
        }

        private bool IsAlreadyActive { get; set; } = false;

        public static void Access(List<Weapon> weapons, Action<int> selectWeapon)
        {
            EventManager.TriggerEvent(OpenWeaponSelection, selectWeapon, weapons);
        }

        private void CloseMenu()
        {
            _weaponSlots.ApplyAction(w =>
            {
                if (IsAlreadyActive)
                {
                    w.icon.sprite = null;


                    w.gameObject.SetActive(false);
                }
            });
            _canvas.gameObject.SetActive(false);
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, false);
            IsAlreadyActive = false;
        }

        private void OpenMenu(Action<int> selectWeapon, List<Weapon> weaponLibrary)
        {
            if (IsAlreadyActive)
            {
                CloseMenu();
                return;
            }

            _canvas.gameObject.SetActive(true);
            int index = 0;
            IsAlreadyActive = true;
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, true);
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
                        EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, false);
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
    }
}