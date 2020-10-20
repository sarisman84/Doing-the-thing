using System;
using System.Collections.Generic;
using Extensions.InputExtension;
using Player.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

namespace Player
{
    public class WeaponController
    {
        [SerializeReference] public BaseWeapon currentWeapon;


        private InputActionReference _aimSightsInput, _primaryFireInput, _secondaryFireInput;


        public WeaponController(InputActionReference aimSights, InputActionReference primaryFire,
            InputActionReference secondaryFire, FirstPersonController controller)
        {
            _aimSightsInput = aimSights;
            _primaryFireInput = primaryFire;
            _secondaryFireInput = secondaryFire;


            controller.onUpdateCallback += LocalUpdate;

            currentWeapon =
                new TestingWeapon(controller.playerCamera.transform.GetComponentInChildren<WeaponVisualiser>(),
                    controller.playerCamera, controller.transform.GetComponentInChildren<HudManager>());
        }


        void LocalUpdate()
        {
            if (currentWeapon == null) return;


            if (_primaryFireInput.GetInputValue<bool>())
            {
                currentWeapon.PrimaryFire();
            }
        }
    }
}