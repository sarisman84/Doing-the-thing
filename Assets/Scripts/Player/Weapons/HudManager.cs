using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Weapons
{
    public class HudManager : MonoBehaviour
    {
        public TMP_Text ammoCounter;
        public Image weaponIcon;


        public TMP_Text currencyCounter;


        public float CurrencyCounter
        {
            set => currencyCounter.text = value.ToString(CultureInfo.InvariantCulture);
        }
        public void UpdateAmmoCounter(int currentAmmo, int maxAmmo)
        {
            ammoCounter.text = $"{maxAmmo}/{currentAmmo}";
        }

        public void SetWeaponIcon(Sprite icon)
        {
            if (icon.Equals(null)) return;
            weaponIcon.sprite = icon;
        }
    }
}