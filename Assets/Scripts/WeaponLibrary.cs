using System;
using System.Collections.Generic;
using System.Linq;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts
{
    public static class WeaponLibrary
    {
        public static readonly Dictionary<string, Weapon> GlobalWeaponLibrary = new Dictionary<string, Weapon>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnGameStart()
        {
            GlobalWeaponLibrary.Add("default_gun",
                new Weapon("Blaster", 0.25f, o => WeaponFireType.HitScan(o, 5f, f => WeaponHitType.DealDamage(f, 10)),
                    Resources.Load<GameObject>("Weapons/Model Prefabs/Blaster")));
        }
    }

    [Serializable]
    public class Weapon
    {
        private string m_WeaponName;
        private float m_FireRate = 0;
        private float m_CurrentRate;
        private Action<Transform> m_ONWeaponFire;

        private Transform m_Model;

        public Weapon(string name, float fireRate, Action<Transform> onWeaponFire, GameObject model)
        {
            m_WeaponName = name;
            m_ONWeaponFire = onWeaponFire;
            m_FireRate = fireRate;

            Debug.Log($"Added Weapon {m_WeaponName} to Global Library.");

            Transform clone = Object.Instantiate(model).transform;
            var transform = clone.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            clone.name = clone.name.Replace("(Clone)", "");
            clone.gameObject.SetActive(false);
            m_Model = clone;

            Debug.Log($"Instantiated Weapon Model for {m_WeaponName}.");
        }

        public void UpdateWeaponState(Transform objectToVisualiseWeapon, bool trigger)
        {
            UpdateWeaponModel(objectToVisualiseWeapon);
            m_CurrentRate += Time.deltaTime;
            if (m_CurrentRate >= m_FireRate && trigger)
            {
                m_ONWeaponFire?.Invoke(m_Model.GetChildWithTag("Weapon/BarrelPoint"));
                m_CurrentRate = 0;
            }
        }

        private void UpdateWeaponModel(Transform objectToVisualiseWeapon)
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


    public static class WeaponFireType
    {
        public static void HitScan(Transform transform, float range, Action<DamageableObject> onFireHit, Action<Ray> onFireEffect = null)
        {
            Ray ray = new Ray(transform.position, transform.forward.normalized);
            Physics.Raycast(ray, out var hitInfo, range);
            onFireHit?.Invoke(hitInfo.collider ? hitInfo.collider.GetComponent<DamageableObject>() : null);
            onFireEffect?.Invoke(ray);
        }
    }

    public static class WeaponHitType
    {
        public static void DealDamage(DamageableObject damageableObject, int damage)
        {
            if (damageableObject)
                damageableObject.TakeDamage(damage);
        }
    }

    public static class WeaponFireEffectType
    {
        public static void Beam(Ray ray, Color beamColor)
        {
            
        }
    }
}