using System;
using System.Collections.Generic;
using System.Linq;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts
{
    [Serializable]
    public class Weapon
    {
        private string m_WeaponName;
        private Sprite m_WeaponIcon;
        private int m_WeaponMaxAmmo;

        private int m_WeaponCurrentAmmo;

        private float m_FireRate;
        private float m_CurrentRate;
        public Action<Transform> onWeaponFire;
        private Transform m_Model;

        public string Name => m_WeaponName;
        public Sprite Icon => m_WeaponIcon;
        public int MaxAmmo => m_WeaponMaxAmmo;
        public int CurrentAmmo => m_WeaponCurrentAmmo;

        public Weapon(string name, string weaponIconName, int maxAmmo, float fireRate, Action<Transform> weaponFire,
            GameObject model)
        {
            m_WeaponName = name;
            m_FireRate = fireRate;
            onWeaponFire = weaponFire;
            try
            {
                m_WeaponIcon = Resources.Load<Sprite>($"Weapons/Icons/{weaponIconName}");
                int id = m_WeaponIcon.GetInstanceID();
                Debug.Log($"Init: Fetehced Weapon Icon for {m_WeaponName} (ID: {id})");
            }
            catch (Exception e)
            {
                Debug.Log($"Init: Couldnt find Weapon Icon for {m_WeaponName}. Skipped.");
            }

            m_WeaponMaxAmmo = maxAmmo;
            m_WeaponCurrentAmmo = maxAmmo;
            try
            {
                Transform clone = Object.Instantiate(model).transform;
                var transform = clone.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                clone.name = name;
                clone.gameObject.SetActive(false);
                m_Model = clone;
            }
            catch (Exception e)
            {
                Debug.Log("Init: Couldnt create model, skipping");
            }

            Debug.Log($"Init: Instantiated Weapon Model for {m_WeaponName}.");
        }

        public void UpdateWeaponState(Transform objectToVisualiseWeapon, bool trigger)
        {
            UpdateWeaponModel(objectToVisualiseWeapon);
            m_CurrentRate += Time.deltaTime;
            if (m_CurrentRate >= m_FireRate && trigger && m_WeaponCurrentAmmo > 0)
            {
                onWeaponFire?.Invoke(m_Model.GetChildWithTag("Weapon/BarrelPoint"));
                m_CurrentRate = 0;
                m_WeaponCurrentAmmo--;
            }
        }

        public int AddAmmo(int amount)
        {
            m_WeaponCurrentAmmo += amount;
            int difference = m_WeaponCurrentAmmo - m_WeaponMaxAmmo;
            m_WeaponCurrentAmmo = Mathf.Clamp(m_WeaponCurrentAmmo, 0, m_WeaponMaxAmmo);
            return difference;
        }

        public void UpdateWeaponModel(Transform objectToVisualiseWeapon)
        {
            List<Transform> currentModels = objectToVisualiseWeapon.GetChildren().ToList();
            foreach (var current in currentModels)
            {
                current.gameObject.SetActive(false);
            }

            Transform existingModel = currentModels.Find(m => m.name.Contains(m_Model.name));
            if (existingModel)
            {
                existingModel.gameObject.SetActive(true);
            }
            else
            {
                m_Model.gameObject.SetActive(true);
                m_Model.SetParent(objectToVisualiseWeapon);
                m_Model.localPosition = Vector3.zero;
                m_Model.localRotation = Quaternion.identity;
            }
        }
    }
}