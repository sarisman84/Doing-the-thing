using System;
using Interactivity;
using Player;
using Player.Weapons;
using UnityEngine;
using Utility;

namespace UI
{
    public class WeaponShop : MonoBehaviour, IInteractable
    {
        public const string SetShopActiveEvent = "Player_SetShopActive";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameLoad()
        {
            WeaponShop shop = GameObject.FindObjectOfType<WeaponShop>();
            if (shop.Equals(null)) return;
            EventManager.AddListener(SetShopActiveEvent, value => shop.SetShopActive((bool) value));
        }

        private Canvas _uiShop;
        public bool IsShopActive => _uiShop.gameObject.activeInHierarchy;

        private void Awake()
        {
            _uiShop = GameObject.FindGameObjectWithTag("Player Shop").GetComponent<Canvas>();
            _uiShop.gameObject.SetActive(false);
        }


        public void OnInteract()
        {
            EventManager.TriggerEvent(SetShopActiveEvent, true);
        }

        private object SetShopActive(bool value)
        {
            EventManager.TriggerEvent(PlayerController.SetCursorActiveEvent, value);
            _uiShop.gameObject.SetActive(value);
            return null;
        }
    }
}