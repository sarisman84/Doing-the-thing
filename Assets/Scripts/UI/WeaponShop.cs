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

namespace UI
{
    public class WeaponShop : MonoBehaviour
    {
        private bool _isShopOpen = false;

        public Transform weaponRenderer;
        public Transform buttonHolder;
        private List<ShopButton> _shopSlots = new List<ShopButton>();
        public TMP_Text description;
        public new TMP_Text name;
        public TMP_Text priceCounter;
        public PlayerController playerController;


        private static CustomEvent _openShopEvent;
        private static CustomEvent _closeShopEvent;
        private static CustomEvent _getShopEvent;


        Dictionary<int, GameObject> _weaponModels = new Dictionary<int, GameObject>();

        public static void Open(Collider caller)
        {
            Open(caller.gameObject, caller.GetComponent<WeaponController>());
        }

        public static void Open(GameObject caller, WeaponController controller)
        {
            if (_openShopEvent != null)
                _openShopEvent.OnInvokeEvent(caller, controller.weaponLibrary);
        }

        public static void Close(GameObject caller)
        {
            if (_closeShopEvent != null)
                _closeShopEvent.OnInvokeEvent(caller);
        }

        public static bool IsActive(GameObject caller)
        {
            if (_getShopEvent != null)
                return (bool) _getShopEvent.OnInvokeEvent(caller, null);
            return false;
        }

        private void Awake()
        {
            _openShopEvent =
                CustomEvent.CreateEvent<Action<List<Weapon>>>(ref _openShopEvent, OpenShop,
                    playerController.gameObject);
            _closeShopEvent =
                CustomEvent.CreateEvent<Action>(ref _closeShopEvent, CloseShop, playerController.gameObject);
            _getShopEvent =
                CustomEvent.CreateEvent<Func<bool>>(ref _getShopEvent, IsShopActive, playerController.gameObject);


            SetupShopButtons(Weapon.GetAllWeapons().OrderBy(c => c.price).ToList());

            gameObject.SetActive(false);
            _weaponModels.ApplyAction(w => w.Value.SetActive(false));
        }

        private void SetupShopButtons(List<Weapon> foundWeapons)
        {
            foreach (var var in foundWeapons)
            {
                ShopButton button = Instantiate(Resources.Load<ShopButton>("UI/Shop Slot"), buttonHolder);
                button.ShopIcon = var.weaponIcon;
                button.ID = var.GetInstanceID();
                button.OnEnteringUIElement += element => SetWeaponInformationToButton(var);
                button.OnExitingUIElement += element => ResetInformation();
                button.OnClickUIElement += () => BuyItem(var);
                _shopSlots.Add(button);
                button.gameObject.SetActive(false);

                GameObject model = Instantiate(var.WeaponModelPrefab, weaponRenderer.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                _weaponModels.Add(var.GetInstanceID(), model);
            }
        }


        public void OpenShop(List<Weapon> library)
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, true);

            gameObject.SetActive(true);
            UpdateShop(library);

            _isShopOpen = true;
        }

        public bool IsShopActive()
        {
            return _isShopOpen;
        }

        private void UpdateShop(List<Weapon> library)
        {
            int index = 0;
            _shopSlots.ApplyAction(s =>
            {
                s.gameObject.SetActive(false);
                if (index < library.Count && (s.ID == library[index].GetInstanceID() || library[index].price == -1))
                {
                    Debug.Log($"{library[index].name} already exists in player's inventory.");
                    index++;
                    return;
                }

                index++;
                s.gameObject.SetActive(true);
            });
        }

        public void CloseShop()
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, false);
            gameObject.SetActive(false);

            _isShopOpen = false;
        }

        private void BuyItem(Weapon weapon)
        {
            if ((bool) EventManager.TriggerEvent("Player_BuyWeapon", weapon.name))
            {
                _shopSlots.Find(s => s.ID == weapon.GetInstanceID()).gameObject.SetActive(false);
                ResetInformation();
            }
        }

        private void ResetInformation()
        {
            description.text = "";
            name.text = "";
            priceCounter.text = "";
            _weaponModels.ApplyAction(w => w.Value.gameObject.SetActive(false));
        }

        private void SetWeaponInformationToButton(Weapon weapon)
        {
            name.text = weapon.name;
            description.text = weapon.description;
            priceCounter.text = weapon.price.ToString();
            _weaponModels.ApplyAction(w =>
            {
                w.Value.gameObject.SetActive(false);

                if (w.Key == weapon.GetInstanceID())
                {
                    w.Value.gameObject.SetActive(true);
                }
            });
        }
    }
}