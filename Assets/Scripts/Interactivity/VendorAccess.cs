using System;
using Interactivity.Vendor;
using Player;
using UnityEngine;
using Utility;

namespace Interactivity
{
    public class VendorAccess : MonoBehaviour, IInteractable
    {
        public string proximityMessage;
        private OutlineManager _manager;

        private void Awake()
        {
            _manager = GetComponent<OutlineManager>();
        }

        public void OnInteract(PlayerController owner)
        {
            EventManager.TriggerEvent("Shop_OpenShop", owner.GetComponent<WeaponController>().weaponLibrary);
        }

        public void OnProximityEnter()
        {
            _manager.SetOutlineActive(true);
        }

        public void OnProximityExit()
        {
            _manager.SetOutlineActive(false);
        }

        public InteractionInput InputType { get; } = InteractionInput.Press;
        public bool NeedToLookAtInteractable { get; } = false;
    }
}