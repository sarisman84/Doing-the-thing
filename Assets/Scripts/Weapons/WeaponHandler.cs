using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform parentObjectToVisualiseWeaponsWith;

        [Space] public InputActionReference fireInputReference;
        public InputActionReference weaponSelectionInputReference;

        private readonly List<Weapon> m_LocalWeaponLibrary = new List<Weapon>();
        private int currentWeapon = 0;

        private void Start()
        {
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_gun"]);
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_grenade"]);
        }

        private void Update()
        {
            SelectWeapon((int)weaponSelectionInputReference.action.ReadValue<float>());
            UseCurrentWeapon(currentWeapon, m_LocalWeaponLibrary);
        }

        private void UseCurrentWeapon(int index, List<Weapon> weapons)
        {
            weapons[index].UpdateWeaponState(parentObjectToVisualiseWeaponsWith,
                fireInputReference.action.ReadValue<float>() > 0);
        }


        private void SelectWeapon(int input)
        {
            currentWeapon += input;
            currentWeapon = currentWeapon < 0 ? m_LocalWeaponLibrary.Count - 1 :
                currentWeapon >= m_LocalWeaponLibrary.Count ? 0 : currentWeapon;
        }
    }
}