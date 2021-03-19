﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Events;
using Interactivity.Pickup;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using Utility;
using static Player.InputController;
using CustomEvent = Interactivity.Events.CustomEvent;
using Debug = UnityEngine.Debug;
using static Player.InputController.KeyCode;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        [HideInInspector] public Weapon currentWeapon;
        public List<Weapon> weaponLibrary;
        [HideInInspector] public PlayerController player;
        private InteractionController controller;

        public void Start()
        {
            player = GetComponent<PlayerController>();

            weaponLibrary = new List<Weapon>();


            player.ONUpdateCallback += LocalUpdate;

            AddWeaponToLibrary(Weapon.GetWeaponViaName(gameObject, "Starter Weapon"));


            SelectWeapon(0);
        }

        private void OnEnable()
        {
            // EventManager.AddListener<Func<string, bool>>("Player_BuyWeapon", OnWeaponPurchace);
            // EventManager.AddListener<Action<string>>("Player_AddWeapon", value =>
            // {
            //     AddWeaponToLibrary(Weapon.GetWeaponViaName(gameObject, value));
            //     SelectWeapon(weaponLibrary.Count - 1);
            // });

            if (controller)
                controller.ONDetectionEvent += PickupAmmo;
        }

        private void PickupAmmo(Collider obj)
        {
            IPickup pickup = obj.GetComponent<IPickup>();

            if (pickup != null)
            {
                pickup.OnPickup(gameObject);
               
            }
        }

        private void OnDisable()
        {
            // EventManager.RemoveListener<Func<string, bool>>("Player_BuyWeapon", OnWeaponPurchace);
            // EventManager.RemoveListener<Action<string>>("Player_AddWeapon", value =>
            // {
            //     AddWeaponToLibrary(Weapon.GetWeaponViaName(gameObject, value));
            //     SelectWeapon(weaponLibrary.Count - 1);
            // });
            InteractionController controller = InteractionController.GetInteractionController(gameObject);
            if (controller)
                controller.ONDetectionEvent -= PickupAmmo;
        }

        public void SelectWeapon(int index)
        {
            if (index >= 0 && index < weaponLibrary.Count)
            {
                currentWeapon = WeaponExtensions.SwapWeaponTo(weaponLibrary, weaponLibrary[index]);
            }
        }

        #region Old Code

        // _weaponVisualiser.SetWeaponModel(this, currentWeapon);
        // // EventManager.TriggerEvent(HeadsUpDisplay.UpdateWeaponIcon, currentWeapon.icon);
        // // EventManager.TriggerEvent(HeadsUpDisplay.UpdateAmmoCounter, currentWeapon);
        //
        // HeadsUpDisplay.UpdateWeaponIconUI(gameObject, currentWeapon.icon);
        // HeadsUpDisplay.UpdateWeaponAmmoUI(gameObject, currentWeapon);

        #endregion

        public void AddWeaponToLibrary(Weapon newWeapon)
        {
            if (!weaponLibrary.Contains(newWeapon))
            {
                newWeapon.Setup(gameObject);
                weaponLibrary.Add(newWeapon);
                SelectWeapon(weaponLibrary.Count - 1);
            }
        }


        void LocalUpdate()
        {
            if (player.Input.GetKeyDown(WeaponSelect) && weaponLibrary.Count > 1)
            {
                WeaponSelectMenu.Open(gameObject, SelectWeapon, weaponLibrary);
            }

            if (player.PlayerCamera.CameraLocked) return;

            if (!currentWeapon)
            {
                Debug.Log("No Weapon found");
                return;
            }

            Debug.Log(currentWeapon.TriggerFire(player.Input.GetKey(Attack)));

            if (!controller)
            {
                controller = InteractionController.GetInteractionController(gameObject);
                controller.ONDetectionEvent += PickupAmmo;
            }
        }


        public void ResupplyWeapon(Weapon targetWeapon)
        {
            weaponLibrary.Find(w => w.GetInstanceID() == targetWeapon.GetInstanceID())
                .AddAmmo((int) (targetWeapon.maxAmmo - targetWeapon.currentAmunition));
            HeadsUpDisplay.UpdateWeaponAmmoUI(gameObject, currentWeapon);
        }
    }
}