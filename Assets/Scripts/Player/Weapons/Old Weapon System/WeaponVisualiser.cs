using System;
using System.Collections.Generic;
using Extensions;
using Player.Weapons.NewWeaponSystem;
using UnityEngine;
using CustomEvent = Interactivity.Events.CustomEvent;

namespace Player.Weapons
{
    public class WeaponVisualiser : MonoBehaviour
    {
        public PlayerController owner;

        private int _selectedModel;

        private static CustomEvent _onSetWeaponModel;

        private void Awake()
        {
            _onSetWeaponModel = _onSetWeaponModel
                ? _onSetWeaponModel
                : CustomEvent.CreateEvent<Action<List<Weapon>, Weapon>>(ref _onSetWeaponModel, SetWeaponModel,
                    owner.gameObject);
        }

        public static void UpdateWeaponModel(GameObject owner, List<Weapon> library, Weapon currentWeapon)
        {
            if (_onSetWeaponModel)
                _onSetWeaponModel.OnInvokeEvent(owner, library, currentWeapon);
        }

        private void ChangeWeaponModel(List<Weapon> library, Weapon selectedWeapon)
        {
            library.ApplyAction(w => w.InstancedWeaponModel.SetActive(false));
            selectedWeapon.InstancedWeaponModel.SetActive(true);
        }

        private void SetWeaponModel(List<Weapon> library, Weapon weapon)
        {
            if (library.Exists(w =>
                w == weapon && weapon.InstancedWeaponModel &&
                weapon.InstancedWeaponModel.transform.parent == transform))
            {
                ChangeWeaponModel(library, weapon);
                return;
            }

            weapon.InstancedWeaponModel = Instantiate(weapon.WeaponModelPrefab, transform);
            weapon.InstancedWeaponModel.transform.SetParent(transform);
            weapon.InstancedWeaponModel.transform.localPosition = Vector3.zero;
            weapon.InstancedWeaponModel.transform.localRotation = Quaternion.identity;
            weapon.InstancedWeaponModel.SetActive(false);

            ChangeWeaponModel(library, weapon);
        }
    }
}