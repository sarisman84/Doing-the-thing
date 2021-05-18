using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts
{
    public class WeaponHandler : MonoBehaviour
    {
        public Transform parentObjectToVisualiseWeaponsWith;

        [Space] public InputActionReference fireInputReference;
        public InputActionReference weaponSelectionInputReference;

        [Header("Weapon UI Settings")] public TMP_Text weaponAmmoCounter;
        public Image weaponIconDisplayer;

        private readonly List<Weapon> m_LocalWeaponLibrary = new List<Weapon>();
        private int m_CurrentWeapon = 0;

        private void Start()
        {
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_gun"]);
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_grenade"]);
            m_LocalWeaponLibrary.Add(WeaponLibrary.GlobalWeaponLibrary["default_rocket_launcher"]);
        }

        private void Update()
        {
            SelectWeapon((int) weaponSelectionInputReference.action.ReadValue<float>());
            UseCurrentWeapon(m_CurrentWeapon, m_LocalWeaponLibrary);
            weaponAmmoCounter.text = DisplayAmmoCounter(m_LocalWeaponLibrary[m_CurrentWeapon]);
        }

        private void UseCurrentWeapon(int index, List<Weapon> weapons)
        {
            weapons[index].UpdateWeaponState(parentObjectToVisualiseWeaponsWith,
                fireInputReference.action.ReadValue<float>() > 0);
        }


        private void SelectWeapon(int input)
        {
            m_CurrentWeapon += input == 0 ? 0 : (int) Mathf.Sign(input);
            m_CurrentWeapon = m_CurrentWeapon < 0 ? m_LocalWeaponLibrary.Count - 1 :
                m_CurrentWeapon >= m_LocalWeaponLibrary.Count ? 0 : m_CurrentWeapon;
            DisplayWeaponIcon(m_LocalWeaponLibrary[m_CurrentWeapon]);
        }


        private string DisplayAmmoCounter(Weapon weapon)
        {
            return $"{weapon.CurrentAmmo}/{weapon.MaxAmmo}";
        }

        private void DisplayWeaponIcon(Weapon weapon)
        {
            weaponIconDisplayer.sprite = weapon.Icon;
        }

        public bool AddAmmoToWeapon(string weaponID)
        {
            Weapon weapon = m_LocalWeaponLibrary.Find(w => w.ID == weaponID);
            if (weapon == null) return false;
            return  weapon.ReplenishAmmo() != 0;;
        }
    }
}