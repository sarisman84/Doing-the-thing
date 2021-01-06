using System;
using System.Collections.Generic;
using Extensions;
using Interactivity.Events;
using Player;
using Player.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Object = UnityEngine.Object;

namespace UI
{
    public class WeaponShop : MonoBehaviour
    {
        public static bool isShopOpen;


        public Transform weaponRenderer;
        public Transform buttonHolder;
        private List<ShopButton> _shopSlots = new List<ShopButton>();
        public TMP_Text description;
        public new TMP_Text name;
        public TMP_Text priceCounter;
        public PlayerController playerController;


        private static InstanceEvent _openShopEvent;
        private static InstanceEvent _closeShopEvent;


        Dictionary<Guid, GameObject> _weaponModels = new Dictionary<Guid, GameObject>();


        public static void Open(Collider caller)
        {
            Open(caller.gameObject, caller.GetComponent<WeaponController>());
        }

        public static void Open(GameObject caller, WeaponController controller)
        {
            _openShopEvent?.OnInvokeEvent(caller, controller.weaponLibrary);
        }

        public static void Close(GameObject caller)
        {
            _closeShopEvent?.OnInvokeEvent(caller);
        }

        private void Awake()
        {
            InstanceEvent.CreateEvent<Action<List<Weapon>>>(ref _openShopEvent, playerController.gameObject, OpenShop);
            InstanceEvent.CreateEvent<Action>(ref _closeShopEvent, playerController.gameObject, CloseShop);


            foreach (var var in WeaponManager.globalWeaponLibrary)
            {
                ShopButton button = Instantiate(Resources.Load<ShopButton>("UI/Shop Slot"), buttonHolder);
                button.ShopIcon = var.Value.icon;
                button.ID = var.Value.ID;
                button.OnEnteringUIElement += element => SetWeaponInformationToButton(var.Value);
                button.OnExitingUIElement += element => ResetInformation();
                button.OnClickUIElement += () => BuyItem(var.Value);
                _shopSlots.Add(button);
                button.gameObject.SetActive(false);

                GameObject model = Instantiate(var.Value.model, weaponRenderer.transform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;

                _weaponModels.Add(var.Value.ID, model);
            }

            gameObject.SetActive(false);
            _weaponModels.ApplyAction(w => w.Value.SetActive(false));
        }


        public void OpenShop(List<Weapon> library)
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, false);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, true);

            gameObject.SetActive(true);
            UpdateShop(library);

            isShopOpen = true;
        }

        private void UpdateShop(List<Weapon> library)
        {
            int index = 0;
            _shopSlots.ApplyAction(s =>
            {
                s.gameObject.SetActive(false);
                if (index < library.Count && s.ID == library[index].ID)
                {
                    Debug.Log($"{library[index].name} already exists in player's inventory.");
                    index++;
                    return;
                }

                s.gameObject.SetActive(true);
            });
        }

        public void CloseShop()
        {
            EventManager.TriggerEvent(InputListener.SetPlayerMovementInputActiveState, true);
            EventManager.TriggerEvent(CameraController.SetCursorActiveEvent, false);
            gameObject.SetActive(false);

            isShopOpen = false;
        }

        private void BuyItem(Weapon weapon)
        {
            if ((bool) EventManager.TriggerEvent("Player_BuyWeapon", weapon.name))
            {
                _shopSlots.Find(s => s.ID == weapon.ID).gameObject.SetActive(false);
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

                if (w.Key == weapon.ID)
                {
                    w.Value.gameObject.SetActive(true);
                }
            });
        }
    }
}