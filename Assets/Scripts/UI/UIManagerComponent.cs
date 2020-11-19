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
    public class UIManagerComponent : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = new UIManager("Weapon Shop", "Weapon Select");
        }
    }
    public class UIManager
    {
        private WeaponSelectMenu _weaponSelectMenu;
        private WeaponShop _weaponShop;
        
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

        Dictionary<Guid,GameObject> _weaponModels = new Dictionary<Guid, GameObject>();
        private bool _isAlreadyActive = false;

        public WeaponShop(Canvas asset)
        {
            _weaponShop = asset;
            _shopSlots = new List<ShopButton>();
            Transform buttonHolder = asset.transform.GetChildWithTag("HUD/Shop/Slots");
            _description = asset.transform.GetChildWithTag("HUD/Shop/Description").GetComponent<TMP_Text>();
            _name = asset.transform.GetChildWithTag("HUD/Shop/Name").GetComponent<TMP_Text>();
            _priceCounter = asset.transform.GetChildWithTag("HUD/Shop/Price").GetComponent<TMP_Text>();
            GameObject weaponRenderer = GameObject.FindGameObjectWithTag("HUD/Shop/Renderer");
            weaponRenderer = weaponRenderer == null ? new GameObject("Temp") : weaponRenderer; 

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

                GameObject model = Object.Instantiate(var.Value.model, weaponRenderer.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                _weaponModels.Add(var.Value.ID,model);
            }

            _weaponShop.gameObject.SetActive(false);
            _weaponModels.ApplyAction(w => w.Value.SetActive(false));

            EventManager.AddListener<Action<List<Weapon>>>("Shop_OpenShop", OpenShop);
            EventManager.AddListener<Action>(CloseShop, _CloseShop);
        }


        private void OpenShop(List<Weapon> library)
        {
           
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, true);

            _weaponShop.gameObject.SetActive(true);
            UpdateShop(library);
            _isAlreadyActive = true;
        }

        private void UpdateShop(List<Weapon> library)
        {
            int index = 0;
            _shopSlots.ApplyAction(s =>
            {
                s.gameObject.SetActive(false);
                if (index < library.Count && s.ID == library[index].ID)
                {
                    index++;
                    return;
                }

                s.gameObject.SetActive(true);
            });
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
            EventManager.TriggerEvent("Player_BuyWeapon", weapon.name);
            _shopSlots.Find(s => s.ID == weapon.ID).gameObject.SetActive(false);
            ResetInformation();
        }

        private void ResetInformation()
        {
            _description.text = "";
            _name.text = "";
            _priceCounter.text = "";
            _weaponModels.ApplyAction(w => w.Value.gameObject.SetActive(false));
        }

        private void SetWeaponInformationToButton(Weapon weapon)
        {
            _name.text = weapon.name;
            _description.text = weapon.description;
            _priceCounter.text = weapon.price.ToString();
            _weaponModels.ApplyAction(w =>
            {
                w.Value.gameObject.SetActive(false);

                if (w.Key == weapon.ID)
                {
                    w.Value.gameObject.SetActive(true);
                }
            });
        }
    }
}