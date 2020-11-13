using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player;
using Player.Weapons;
using TMPro;
using UnityEngine;
using Utility;
using Object = UnityEngine.Object;

namespace UI
{
    public class UIManager
    {
        private WeaponSelectMenu _weaponSelectMenu;
        private WeaponShop _weaponShop;
        public static UIManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameLoad()
        {
            Instance = new UIManager("Weapon Shop", "Weapon Select");
        }

        public UIManager(params string[] uiAssets)
        {
            Dictionary<string, Canvas> uiCanvases = new Dictionary<string, Canvas>();
            foreach (var asset in uiAssets)
            {
                uiCanvases.Add(asset, Object.Instantiate(Resources.Load<Canvas>($"UI/{asset}")));
            }

            _weaponSelectMenu = new WeaponSelectMenu(uiCanvases["Weapon Select"]);
            _weaponShop = new WeaponShop(uiCanvases["Weapon Shop"]);
        }
    }

    public class WeaponShop
    {
        public const string CloseShop = "Shop_CloseShop";
        List<ShopButton> _shopSlots;
        private Canvas _weaponShop;
        private TMP_Text _description;
        private TMP_Text _name;
        private TMP_Text _priceCounter;
        private bool _isAlreadyActive = false;

        public WeaponShop(Canvas asset)
        {
            _weaponShop = asset;
            _shopSlots = new List<ShopButton>();
            Transform buttonHolder = asset.transform.GetChildWithTag("HUD/Shop/Slots");
            _description = asset.transform.GetChildWithTag("HUD/Shop/Description").GetComponent<TMP_Text>();
            _name = asset.transform.GetChildWithTag("HUD/Shop/Name").GetComponent<TMP_Text>();
            _priceCounter = asset.transform.GetChildWithTag("HUD/Shop/Price").GetComponent<TMP_Text>();
            foreach (var var in WeaponManager.globalWeaponLibrary)
            {
                ShopButton button = Object.Instantiate(Resources.Load<ShopButton>("UI/Shop Slot"), buttonHolder);
                button.ShopIcon = var.Value.icon;
                button.ID = var.Value.ID;
                button.OnEnteringUIElement += element => SetWeaponInformationToButton(var.Value);
                button.OnExitingUIElement += element => ResetInformation();
                button.OnClickUIElement += () => BuyItem(var.Value);
                _shopSlots.Add(button);
                button.gameObject.SetActive(false);
            }

            _weaponShop.gameObject.SetActive(false);


            EventManager.AddListener<Action<List<Weapon>>>("Shop_OpenShop", OpenShop);
            EventManager.AddListener<Action>(CloseShop, _CloseShop);
        }


        private void OpenShop(List<Weapon> library)
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, true);

            _weaponShop.gameObject.SetActive(true);
            int index = 0;
            _shopSlots.ApplyAction(s =>
            {
                if (index < library.Count && s.ID == library[index].ID)
                {
                    index++;
                    return;
                }

                s.gameObject.SetActive(true);
            });
            _isAlreadyActive = true;
        }

        private void _CloseShop()
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, false);
            _weaponShop.gameObject.SetActive(false);
            _isAlreadyActive = false;
        }

        private void BuyItem(Weapon weapon)
        {
            EventManager.TriggerEvent("Player_BuyWeapon", weapon);
        }

        private void ResetInformation()
        {
            _description.text = "";
            _name.text = "";
            _priceCounter.text = "";
        }

        private void SetWeaponInformationToButton(Weapon weapon)
        {
            _name.text = weapon.name;
            _description.text = weapon.description;
            _priceCounter.text = weapon.price.ToString();
        }
    }
}