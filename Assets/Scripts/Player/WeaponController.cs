using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using Utility;
using Debug = System.Diagnostics.Debug;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        public Weapon currentWeapon;
        public List<Weapon> weaponLibrary;


        [HideInInspector] public PlayerController player;
        private WeaponVisualiser _weaponVisualiser;
       


        public void Start()
        {
            player = GetComponent<PlayerController>();

            weaponLibrary = new List<Weapon>();
            
            _weaponVisualiser =
              Camera.main.transform.GetComponentInChildren<WeaponVisualiser>();


            player.ONUpdateCallback += LocalUpdate;

            AddWeaponToLibrary(WeaponManager.globalWeaponLibrary["Test_Pistol"]);
            

            SelectWeapon(0);
        }

        private void OnEnable()
        {
            EventManager.AddListener<Action<string>>("Player_BuyWeapon", OnWeaponPurchace);
            EventManager.AddListener<Action<string>>("Player_AddWeapon", value =>
            {
                if (WeaponManager.globalWeaponLibrary.ContainsKey(value))
                {
                    AddWeaponToLibrary(WeaponManager.globalWeaponLibrary[value]);
                    SelectWeapon(weaponLibrary.Count - 1);
                }
            });
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<Action<string>>("Player_BuyWeapon", OnWeaponPurchace);
            EventManager.RemoveListener<Action<string>>("Player_AddWeapon", value =>
            {
                if (WeaponManager.globalWeaponLibrary.ContainsKey(value))
                {
                    AddWeaponToLibrary(WeaponManager.globalWeaponLibrary[value]);
                    SelectWeapon(weaponLibrary.Count - 1);
                }
            });
        }

        public void SelectWeapon(int index)
        {
            if (index >= 0 && index < weaponLibrary.Count)
            {
                currentWeapon = weaponLibrary[index];
                _weaponVisualiser.SetWeaponModel(this, currentWeapon);
                EventManager.TriggerEvent(HudManager.UpdateWeaponIcon, currentWeapon.icon);
                EventManager.TriggerEvent(HudManager.UpdateAmmoCounter, currentWeapon);
            }
        }

        public void AddWeaponToLibrary(Weapon newWeapon)
        {
            if (!weaponLibrary.Contains(newWeapon))
                weaponLibrary.Add(newWeapon);
        }


        void LocalUpdate()
        {
            if (InputListener.GetKeyDown(InputListener.KeyCode.WeaponSelect) && weaponLibrary.Count > 1)
            {
                WeaponSelectMenu.Access(weaponLibrary, SelectWeapon);
            }

            if (player.CameraController.CameraLocked) return;

            if (currentWeapon == null) return;

            currentWeapon.OnWeaponPrimaryFire(this);
        }


        void OnWeaponPurchace(string weapon)
        {
            Weapon newWeapon = WeaponManager.globalWeaponLibrary[weapon];
            if ((int) EventManager.TriggerEvent(CurrencyHandler.GetCurrency, "") < newWeapon.price) return;
            EventManager.TriggerEvent(CurrencyHandler.PayCurrency, newWeapon.price);
            AddWeaponToLibrary(newWeapon);
            SelectWeapon(weaponLibrary.Count - 1);
        }
    }
}