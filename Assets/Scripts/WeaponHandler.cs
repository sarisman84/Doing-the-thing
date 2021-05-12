using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform parentObjectToVisualiseWeaponsWith;
        public InputActionReference fireInputReference;

        private readonly List<Weapon> m_LocalWeaponLibrary = new List<Weapon>();
        private readonly int currentWeapon = 0;

        private void Start()
        {
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_gun"]);
        }

        private void Update()
        {
            UseCurrentWeapon(currentWeapon, m_LocalWeaponLibrary);
        }

        private void UseCurrentWeapon(int index, List<Weapon> weapons)
        {
            weapons[index].UpdateWeaponState(parentObjectToVisualiseWeaponsWith,
                fireInputReference.action.ReadValue<float>() > 0);
        }
    }
}