using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Player.Weapons
{
    public class WeaponVisualiser : MonoBehaviour
    {
        private int _selectedModel;

        public void ChangeWeaponModel(WeaponController controller, Weapon selectedWeapon)
        {
            controller.weaponLibrary.ApplyAction(w => w.model.SetActive(false));
            selectedWeapon.model.SetActive(true);
        }

        public void SetWeaponModel(WeaponController controller, Weapon weapon)
        {
            if (controller.weaponLibrary.Exists(w => w.ID == weapon.ID && weapon.model.transform.parent == transform))
            {
                ChangeWeaponModel(controller, weapon);
                return;
            }

            weapon.model = Instantiate(weapon.model, transform);
            weapon.model.transform.SetParent(transform);
            weapon.model.transform.localPosition = Vector3.zero;
            weapon.model.transform.localRotation = Quaternion.identity;
            weapon.model.SetActive(false);

            ChangeWeaponModel(controller, weapon);
        }
    }
}