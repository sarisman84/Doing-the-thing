using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using Weapons.State_Manager;
using Object = UnityEngine.Object;

namespace Scripts.Weapons
{
    [Serializable]
    public class Weapon
    {
        private float m_FireRate;
        private float m_CurrentRate;
        private bool m_Trigger;
        private float m_FiringDuration;
        public Action<Transform> onWeaponFire;
        private Transform m_Model;

        public int AmmoPickupAmmount { get; private set; }
        public string Name { get; private set; }
        public Sprite Icon { get; private set; }
        public int MaxAmmo { get; private set; }
        public int CurrentAmmo { get; private set; }
        public string ID { get; private set; }
        public GameObject AmmoPickupModel { get; private set; }

        public WeaponStateMachine weaponState;


        public Weapon(string name, string string_id, int maxAmmo, int ammoPickupAmm,
            float fireRate, Action<Transform> weaponFire,
            GameObject model)
        {
            Name = name;
            m_FireRate = fireRate;
            m_FiringDuration = m_FireRate / 2f;
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


        private bool IsInCooldown = false;
        private int currentState = 0;
        private float m_CurFireDur;

        public void UpdateWeaponState(MonoBehaviour sceneObject, Transform objectToVisualiseWeapon, bool trigger)
        {
            UpdateWeaponModel(objectToVisualiseWeapon);
            if (currentState == 2)
                m_CurrentRate -= Time.deltaTime;
            else if (currentState == 0)
                m_CurrentRate += Time.deltaTime;

            if (m_CurrentRate >= m_FireRate / 2f && trigger && CurrentAmmo > 0 && currentState == 0)
            {
                onWeaponFire?.Invoke(m_Model.GetChildWithTag("Weapon/BarrelPoint"));
                currentState = 1;
                CurrentAmmo--;
                m_CurrentRate = m_FireRate;
            }

            if (currentState == 1)
            {
                m_CurFireDur += Time.deltaTime;
                if (m_CurFireDur >= m_FiringDuration)
                {
                    m_CurFireDur = 0;
                    currentState = 2;
                }
            }

            if (m_CurrentRate <= 0)
            {
                currentState = 0;
            }

            VisualizeWeaponState(currentState);
        }

        private void VisualizeWeaponState(int state)
        {
            MeshRenderer renderer = m_Model.GetComponentInChildren<MeshRenderer>();
            switch (state)
            {
                case 1:
                    renderer.material.color = Color.red;
                    break;
                case 0:
                    renderer.material.color = Color.green;
                    break;
                case 2:
                    renderer.material.color = Color.yellow;
                    break;
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