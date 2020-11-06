using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;
using Button = UnityEngine.UIElements.Button;

namespace Player.Weapons
{
    public class WeaponSelectMenu : MonoBehaviour
    {
        public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();


        private static WeaponSelectMenu _menu;
        private static bool IsAlreadyActive { get; set; } = false;


        private void Awake()
        {
            if (_menu)
            {
                Destroy(gameObject);
                return;
            }


            weaponSlots.ApplyAction(w => w.gameObject.SetActive(false));

            _menu = this;
        }

        private void OnEnable()
        {
        }

        public static void CloseMenu()
        {
            _menu.weaponSlots.ApplyAction(w =>
            {
                if (IsAlreadyActive)
                {
                    w.icon.sprite = null;


                    w.gameObject.SetActive(false);
                }
            });

            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, false);
            IsAlreadyActive = false;
        }

        public static void OpenMenu(Action<int> selectWeapon, List<Weapon> weaponLibrary)
        {
            if (_menu.Equals(null) || IsAlreadyActive)
            {
                CloseMenu();
                return;
            }

            int index = 0;
            IsAlreadyActive = true;
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, true);
            _menu.weaponSlots.ApplyAction(w => index = ApplyBehaivour(weaponLibrary, selectWeapon, w, index));
        }

        private static int ApplyBehaivour(List<Weapon> weaponLibrary, Action<int> selectWeapon, WeaponSlot w, int index)
        {
            if (index >= 0 && index < weaponLibrary.Count)
            {
                w.gameObject.SetActive(true);
                var index1 = index;
                w.slotButton.onClick.AddListener(() => selectWeapon.Invoke(index1));
                w.slotButton.onClick.AddListener(() =>
                    {
                        _menu.weaponSlots.ApplyAction(b => b.gameObject.SetActive(false));
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