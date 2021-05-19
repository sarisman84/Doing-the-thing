using System;
using System.Collections.Generic;
using System.Linq;
using General_Scripts.Utility.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts
{
    [Serializable]
    public class Weapon
    {
        private float m_FireRate;
        private float m_CurrentRate;
        public Action<Transform> onWeaponFire;
        private Transform m_Model;

        public int AmmoPickupAmmount { get; private set; }
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        public int MaxAmmo { get; private set; }
        public int CurrentAmmo { get; private set; }
        public string ID { get; private set; }
        public GameObject AmmoPickupModel { get; private set; }


        public Weapon(string name, string string_id, int maxAmmo, int ammoPickupAmmount, float fireRate,
            GameObject model)
        {
            Name = name;
            m_FireRate = fireRate;
            ID = string_id;
            AmmoPickupAmmount = ammoPickupAmmount;
            Icon = Resources.Load<Sprite>($"Weapons/Icons/{ID}_icon");
            Icon = Icon ? Icon : Resources.Load<Sprite>("Weapons/Icons/default_gun_icon");
            int id = Icon.GetInstanceID();
            Debug.Log($"Init: Fetched Weapon Icon for {Name} (ID: {id})");
            try
            {
                AmmoPickupModel = Resources.Load<GameObject>($"Weapons/Ammo/Model Prefabs/{string_id}_ammo");
                Debug.Log($"Init: Found Ammo Pickup Model for {Name}. Registering reference.");
            }
            catch (Exception e)
            {
                Debug.Log("Init: Couldnt find Ammo Pickup Model. Skipping.");
            }


            MaxAmmo = maxAmmo;
            CurrentAmmo = maxAmmo;
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

            Debug.Log($"Init: Instantiated Weapon Model for {Name}.");
        }


        public Weapon(string name, string string_id, int maxAmmo, int ammoPickupAmm,
            float fireRate, Action<Transform> weaponFire,
            GameObject model)
        {
            Name = name;
            m_FireRate = fireRate;
            onWeaponFire = weaponFire;
            ID = string_id;
            AmmoPickupAmmount = ammoPickupAmm;
            Icon = Resources.Load<Sprite>($"Weapons/Icons/{ID}_icon");
            Icon = Icon ? Icon : Resources.Load<Sprite>("Weapons/Icons/default_gun_icon");
            int id = Icon.GetInstanceID();
            Debug.Log($"Init: Fetched Weapon Icon for {Name} (ID: {id})");
            try
            {
                AmmoPickupModel = Resources.Load<GameObject>($"Weapons/Ammo/Model Prefabs/{string_id}_ammo");
                Debug.Log($"Init: Found Ammo Pickup Model for {Name}. Registering reference.");
            }
            catch (Exception e)
            {
                Debug.Log("Init: Couldnt find Ammo Pickup Model. Skipping.");
            }


            MaxAmmo = maxAmmo;
            CurrentAmmo = maxAmmo;
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

            Debug.Log($"Init: Instantiated Weapon Model for {Name}.");
        }

        public void UpdateWeaponState(Transform objectToVisualiseWeapon, bool trigger)
        {
            UpdateWeaponModel(objectToVisualiseWeapon);
            m_CurrentRate += Time.deltaTime;
            if (m_CurrentRate >= m_FireRate && trigger && CurrentAmmo > 0)
            {
                onWeaponFire?.Invoke(m_Model.GetChildWithTag("Weapon/BarrelPoint"));
                m_CurrentRate = 0;
                CurrentAmmo--;
            }
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

        public int ReplenishAmmo()
        {
            int difference = CurrentAmmo - MaxAmmo;
            CurrentAmmo += AmmoPickupAmmount;
            CurrentAmmo = Mathf.Clamp(CurrentAmmo, 0, MaxAmmo);
            return difference;
        }
    }
}