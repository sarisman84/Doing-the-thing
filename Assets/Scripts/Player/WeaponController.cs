using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Extensions.InputExtension;
using Player.Weapons;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using Utility;

namespace Player
{
    public class WeaponController
    {
        public Weapon currentWeapon;


        public List<Weapon> weaponLibrary = new List<Weapon>();


        public PlayerController player;
        private WeaponVisualiser _weaponVisualiser;
        public HudManager hudManager;

        public WeaponController(PlayerController player)
        {
            this.player = player;


            _weaponVisualiser = player.playerCamera.transform.GetComponentInChildren<WeaponVisualiser>();

            hudManager = new HudManager();

            player.ONUpdateCallback += LocalUpdate;


            weaponLibrary.Add(WeaponManager.globalWeaponLibrary["Test_Pistol"]);
           
            
            EventManager.AddListener<Action<string>>("Player_BuyWeapon", OnWeaponPurchace);

            
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
            weaponLibrary.Add(newWeapon);
        }


        void LocalUpdate()
        {
            if (InputListener.GetKeyDown(InputListener.KeyCode.WeaponSelect))
            {
                WeaponSelectMenu.Access(weaponLibrary, SelectWeapon);
            }

            if (player.CameraLocked) return;

            if (currentWeapon == null) return;

            currentWeapon.OnWeaponPrimaryFire(this);
        }


        void OnWeaponPurchace(string weapon)
        {
            Weapon newWeapon = WeaponManager.globalWeaponLibrary[weapon];
            if ((int) EventManager.TriggerEvent(CurrencyHandler.GetCurrency) < newWeapon.price) return;
            EventManager.TriggerEvent(CurrencyHandler.PayCurrency, newWeapon.price);
            AddWeaponToLibrary(newWeapon);
            SelectWeapon(weaponLibrary.Count - 1);
        }
    }
}