using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Extensions;
using Interactivity.Events;
using Player;
using Player.Weapons;
using Player.Weapons.NewWeaponSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using CustomEvent = Interactivity.Events.CustomEvent;
using Object = UnityEngine.Object;
using Action = System.Action;

namespace UI
{
    public class WeaponShopMenu : MonoBehaviour
    {
        private static CustomEvent onOpenShopMenu, onCloseShopMenu;


        public PlayerController owner;
        [Space] public ShopButton shopButtonPrefab;
        [Header("Text elements")] public TMP_Text priceCounter;
        public TMP_Text description, title;
        [Header("Model Rendering")] public Transform shopModelParent;


        private List<ShopButton> _shopButtons;
        private bool _isShopOpen;

        private void OnEnable()
        {
            if (_shopButtons == null)
                InitializeShop();


            onOpenShopMenu = CustomEvent.CreateEvent<Action<List<Weapon>>>(OpenMenu, owner.gameObject);
            onCloseShopMenu = CustomEvent.CreateEvent<Func<bool>>(CloseMenu, owner.gameObject);
        }

        private void OnDisable()
        {
            onOpenShopMenu.RemoveEvent<Action<List<Weapon>>>(OpenMenu);
            onCloseShopMenu.RemoveEvent<Func<bool>>(CloseMenu);
        }

        private void InitializeShop()
        {
            _shopButtons = new List<ShopButton>();
            Transform parent = new GameObject("Shop Buttons").transform;

            parent.SetParent(transform);
            foreach (var weapon in Weapon.GetAllWeapons())
            {
                ShopButton button = Instantiate(shopButtonPrefab, parent);
                button.transform.localPosition = Vector3.zero;
                button.transform.rotation = Quaternion.identity;


                button.AssignedWeapon = weapon;
                button.OnEnteringUIElement += element => DisplayInformation(element as ShopButton);

                button.AmmoModel = Instantiate(weapon.ammoType.ammoPrefab, shopModelParent);
                button.AmmoModel.SetActive(false);
                button.WeaponModel = Instantiate(weapon.WeaponModelPrefab, shopModelParent);
                button.WeaponModel.SetActive(false);
            }
        }


        void OpenMenu(List<Weapon> weaponLibrary)
        {
            _isShopOpen = true;
            DisplayShopElements(weaponLibrary);
        }

        private void DisplayShopElements(List<Weapon> weaponLibrary)
        {
            foreach (var button in _shopButtons)
            {
                button.UpdateButton(weaponLibrary);
            }
        }


        bool CloseMenu()
        {
            if (_isShopOpen)
            {
                _isShopOpen = false;
                return true;
            }

            return false;
        }

        void DisplayInformation(ShopButton shopButton)
        {
            if (priceCounter)
                priceCounter.text = shopButton.DisplayPrice();

            if (title)
                title.text = shopButton.DisplayTitle();

            if (description)
                description.text = shopButton.DisplayDecription();
        }


        #region Global Methods

        public static void OpenShop(GameObject owner)
        {
            WeaponController controller = owner.GetComponent<WeaponController>();
            if (onOpenShopMenu && controller != null)
                onOpenShopMenu.OnInvokeEvent(owner, controller.weaponLibrary);
        }

        public static void OpenShop(Collider collider)
        {
            OpenShop(collider.gameObject);
        }

        public static bool CloseShop(GameObject owner)
        {
            if (onCloseShopMenu)
                return (bool)onCloseShopMenu.OnInvokeEvent(owner);
            return false;
        }

        #endregion
    }
}