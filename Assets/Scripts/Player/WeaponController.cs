using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Interactivity.Events;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;
using Debug = UnityEngine.Debug;

namespace Player
{
    public class WeaponController : MonoBehaviour
    {
        [HideInInspector] public Weapon currentWeapon;
        public List<Weapon> weaponLibrary;
        [HideInInspector] public PlayerController player;


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
            EventManager.AddListener<Func<string, bool>>("Player_BuyWeapon", OnWeaponPurchace);
            EventManager.AddListener<Action<string>>("Player_AddWeapon", value =>
            {
                AddWeaponToLibrary(Weapon.GetWeaponViaName(gameObject, value));
                SelectWeapon(weaponLibrary.Count - 1);
            });
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<Func<string, bool>>("Player_BuyWeapon", OnWeaponPurchace);
            EventManager.RemoveListener<Action<string>>("Player_AddWeapon", value =>
            {
                AddWeaponToLibrary(Weapon.GetWeaponViaName(gameObject, value));
                SelectWeapon(weaponLibrary.Count - 1);
            });
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
                weaponLibrary.Add(newWeapon);
        }


        void LocalUpdate()
        {
            if (InputListener.GetKeyDown(InputListener.KeyCode.WeaponSelect) && weaponLibrary.Count > 1)
            {
                //WeaponSelectMenu.Access(weaponLibrary, SelectWeapon);
                WeaponSelectMenu.Open(gameObject, SelectWeapon, weaponLibrary);
            }

            if (player.CameraController.CameraLocked) return;

            if (currentWeapon == null)
            {
                Debug.Log("No Weapon found");
                return;
            }

            Debug.Log(currentWeapon.TriggerFire(InputListener.GetKey(InputListener.KeyCode.Attack)));
        }


        bool OnWeaponPurchace(string weapon)
        {
            Weapon newWeapon = Weapon.GetWeaponViaName(gameObject, weapon);
            if (CurrencyHandler.GetCurrency(gameObject) < newWeapon.price) return false;
            CurrencyHandler.PayCurrency(gameObject, newWeapon.price);
            AddWeaponToLibrary(newWeapon);
            SelectWeapon(weaponLibrary.Count - 1);
            return true;
        }
    }
}