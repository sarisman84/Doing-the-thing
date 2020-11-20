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
    public class WeaponController : MonoBehaviour
    {
        public Weapon currentWeapon;


        public List<Weapon> weaponLibrary = new List<Weapon>();


        [HideInInspector] public PlayerController player;
        private WeaponVisualiser _weaponVisualiser;
        public HudManager hudManager;

        public void Awake()
        {
            player = GetComponent<PlayerController>();


            _weaponVisualiser = player.playerCamera.transform.GetComponentInChildren<WeaponVisualiser>();

            hudManager = new HudManager(this);

            player.ONUpdateCallback += LocalUpdate;


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
                hudManager.SetWeaponIcon(currentWeapon.icon);
                hudManager.UpdateAmmoCounter(currentWeapon);
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

            if (player.CameraLocked) return;

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