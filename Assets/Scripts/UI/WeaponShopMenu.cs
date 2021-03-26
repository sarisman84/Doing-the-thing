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
        private static CustomEvent _onOpenShopMenu, _onCloseShopMenu;


        public PlayerController owner;
        [Space] public ShopButton shopButtonPrefab;
        [Space] public Canvas weaponShopUI;
        [Header("Model Rendering")] public Transform shopModelParent;


        private List<ShopButton> _shopButtons;
        private bool _isShopOpen;

        TMP_Text _priceCounter;
        TMP_Text _description, _title;

        private void OnEnable()
        {
            if (_shopButtons == null)
                InitializeShop();

            weaponShopUI.gameObject.SetActive(false);

            _onOpenShopMenu = CustomEvent.CreateEvent<Action<List<Weapon>>>(OpenMenu, owner.gameObject);
            _onCloseShopMenu = CustomEvent.CreateEvent<Func<bool>>(CloseMenu, owner.gameObject);
        }

        private void OnDisable()
        {
            _onOpenShopMenu.RemoveEvent<Action<List<Weapon>>>(OpenMenu);
            _onCloseShopMenu.RemoveEvent<Func<bool>>(CloseMenu);
        }

        private void InitializeShop()
        {
            _shopButtons = new List<ShopButton>();


            _priceCounter = weaponShopUI.transform.GetChildWithTag("HUD/Shop/Price").GetComponent<TextMeshProUGUI>();
            _title = weaponShopUI.transform.GetChildWithTag("HUD/Shop/Name").GetComponent<TextMeshProUGUI>();
            _description = weaponShopUI.transform.GetChildWithTag("HUD/Shop/Description")
                .GetComponent<TextMeshProUGUI>();


            foreach (var weapon in Weapon.GetAllWeapons())
            {
                ShopButton button = Instantiate(shopButtonPrefab,
                    weaponShopUI.transform.GetChildWithTag("HUD/Shop/Slots"));
                button.transform.localPosition = Vector3.zero;
                button.transform.rotation = Quaternion.identity;


                button.AssignedWeapon = weapon;
                button.OnEnteringUIElement += element => DisplayInformation(element as ShopButton);
                button.OnExitingUIElement += element => ResetInformation(element as ShopButton);
                button.OnClickUIElement += () =>
                {
                    button.BuyItem(owner.gameObject);
                    DisplayShopElements(owner.WeaponController.weaponLibrary);
                    DisplayInformation(button);
                };

                if (weapon.ammoType.ammoPrefab)
                {
                    button.AmmoModel = Instantiate(weapon.ammoType.ammoPrefab, shopModelParent);
                    Destroy(button.AmmoModel.GetComponent<Rigidbody>());
                    button.AmmoModel.SetActive(false);
                }

                button.WeaponModel = Instantiate(weapon.WeaponModelPrefab, shopModelParent);
                button.WeaponModel.SetActive(false);

                _shopButtons.Add(button);
            }
        }


        void OpenMenu(List<Weapon> weaponLibrary)
        {
            weaponShopUI.gameObject.SetActive(true);
            CameraController.SetCursorActive(owner.gameObject, true);
            CameraController.SetCameraInputActive(owner.gameObject, false);
            InputController.SetMovementInputActive(owner.gameObject, false);
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
                weaponShopUI.gameObject.SetActive(false);
                CameraController.SetCursorActive(owner.gameObject, false);
                CameraController.SetCameraInputActive(owner.gameObject, true);
                InputController.SetMovementInputActive(owner.gameObject, true);
                return true;
            }

            return false;
        }

        void DisplayInformation(ShopButton shopButton)
        {
            if (_priceCounter)
                _priceCounter.text = shopButton.DisplayPrice();

            if (_title)
                _title.text = shopButton.DisplayTitle();

            if (_description)
                _description.text = shopButton.DisplayDecription();

            shopButton.DisplayItem();
        }

        private void ResetInformation(ShopButton element)
        {
            element.ResetDisplay();
            _title.text = "";
            _priceCounter.text = "";
            _description.text = "";
        }


        #region Global Methods

        public static void OpenShop(GameObject owner)
        {
            WeaponController controller = owner.GetComponent<WeaponController>();
            if (_onOpenShopMenu && controller != null)
                _onOpenShopMenu.OnInvokeEvent(owner, controller.weaponLibrary);
        }

        public static void OpenShop(Collider collider)
        {
            OpenShop(collider.gameObject);
        }

        public static bool CloseShop(GameObject owner)
        {
            if (_onCloseShopMenu)
                return (bool) _onCloseShopMenu.OnInvokeEvent(owner, null);
            return false;
        }

        #endregion
    }
}