using Player;
using UnityEngine;
using Utility;

namespace Interactivity
{
    public class InteractableEvent : MonoBehaviour, IInteractable
    {
        public string eventName;
        [Space] public string proximityMessage;
        public void OnInteract(PlayerController owner)
        {
            EventManager.TriggerEvent(eventName, owner.GetComponent<WeaponController>().weaponLibrary);
        }

        public void OnProximity()
        {
            // EventManager.TriggerEvent("");
        }
    }
}