using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform barrel;
        public Transform objectToVisualiseWeapon;
        public InputActionReference fireInputReference;
        
        private List<Weapon> localWeaponLibrary = new List<Weapon>();
        private int currentWeapon = 0;

        private void Awake()
        {
            localWeaponLibrary.Add(WeaponLibrary.globalWeaponLibrary["default_gun"]);
        }

        private void Update()
        {
            UseCurrentWeapon(currentWeapon, localWeaponLibrary);
        }

        private void UseCurrentWeapon(int index, List<Weapon> weapons)
        {
            if (weapons.Count < index && weapons.Count != 0)
            {
                weapons[index].UpdateWeaponState(barrel,objectToVisualiseWeapon, fireInputReference.action.ReadValue<float>() > 0);
            }
        }
    }
}