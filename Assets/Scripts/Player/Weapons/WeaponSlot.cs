using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Weapons
{
    public class WeaponSlot : MonoBehaviour
    {
        public Image icon;
        public Button slotButton;

 


        public void SetWeaponSlotIcon(Weapon weapon)
        {
            icon.sprite = weapon.icon;
        }


        public void OnDisable()
        {
            slotButton.onClick.RemoveAllListeners();
        }

  
    }
}