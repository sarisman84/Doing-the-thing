using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Extensions.InputExtension;
using Player.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

namespace Player
{
    public class WeaponController
    {
        public Weapon currentWeapon;


        public List<Weapon> weaponLibrary = new List<Weapon>();


        private InputActionReference _aimSightsInput;
        private readonly InputActionReference _primaryFireInput;
        private InputActionReference _secondaryFireInput, _weaponSelectionInput;
        public FirstPersonController player;
        private WeaponVisualiser _weaponVisualiser;
        public HudManager hudManager;

        public WeaponController(InputActionReference aimSights, InputActionReference primaryFire,
            InputActionReference secondaryFire, InputActionReference weaponSelection, FirstPersonController player)
        {
            _aimSightsInput = aimSights;
            _primaryFireInput = primaryFire;
            _secondaryFireInput = secondaryFire;
            _weaponSelectionInput = weaponSelection;
            this.player = player;


            _weaponVisualiser = player.playerCamera.transform.GetComponentInChildren<WeaponVisualiser>();

            hudManager = player.transform.GetComponentInChildren<HudManager>();

            player.ONUpdateCallback += LocalUpdate;


            weaponLibrary.Add(WeaponManager.globalWeaponLibrary["Test_Pistol"]);
            weaponLibrary.Add(WeaponManager.globalWeaponLibrary["Eliott's Seal Generator"]);
            weaponLibrary.Add(WeaponManager.globalWeaponLibrary["Rocket Launcher"]);

            SelectWeapon(0);
        }

        public void SelectWeapon(int index)
        {
            if (index >= 0 && index < weaponLibrary.Count)
            {
                currentWeapon = weaponLibrary[index];
                _weaponVisualiser.SetWeaponModel(this, currentWeapon);
                hudManager.SetWeaponIcon(currentWeapon.icon);
                hudManager.UpdateAmmoCounter(currentWeapon);
            }
        }

        public void AddWeaponToLibrary(Weapon newWeapon)
        {
            if (weaponLibrary.Find(w => w.ID == newWeapon.ID).Equals(null))
            {
                weaponLibrary.Add(newWeapon);
            }
        }


        void LocalUpdate()
        {
            if (_weaponSelectionInput.GetButtonDown())
            {
                WeaponSelectMenu.OpenMenu(SelectWeapon, weaponLibrary);
            }

            if (player.CameraLocked) return;

            if (currentWeapon == null) return;

            currentWeapon.OnWeaponPrimaryFire(_primaryFireInput, this);
        }
    }
}