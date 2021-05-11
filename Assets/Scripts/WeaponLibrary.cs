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
        public static Dictionary<string, Weapon> globalWeaponLibrary = new Dictionary<string, Weapon>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameStart()
        {
            globalWeaponLibrary.Add("default_gun",
                new Weapon("Blaster", 0.25f, o => WeaponFireType.HitScan(o, 5f, f => WeaponHitType.DealDamage(f, 10)),
                    Resources.Load<GameObject>("Weapon Models/Blaster Model")));
        }
    }

    [Serializable]
    public class Weapon
    {
        private string m_WeaponName;
        private float m_FireRate = 0;
        private float m_CurrentRate;
        private Action<Transform> m_ONWeaponFire;
        private GameObject m_ModelRef;

        public Weapon(string name, float fireRate, Action<Transform> onWeaponFire, GameObject model)
        {
            m_WeaponName = name;
            m_ONWeaponFire = onWeaponFire;
            m_FireRate = fireRate;
            m_ModelRef = model;
        }

        public void UpdateWeaponState(Transform barrel, Transform objectToVisualiseWeapon, bool trigger)
        {
            m_CurrentRate += Time.deltaTime;
            if (m_CurrentRate >= m_FireRate && trigger)
            {
                m_ONWeaponFire?.Invoke(barrel);
                m_CurrentRate = 0;
            }

            UpdateWeaponModel(objectToVisualiseWeapon);
        }

        private void UpdateWeaponModel(Transform objectToVisualiseWeapon)
        {
            if (objectToVisualiseWeapon.childCount == 0)
                return;


            List<Transform> currentModels = objectToVisualiseWeapon.GetChildren().ToList();

            foreach (var current in currentModels)
            {
                current.gameObject.SetActive(false);
            }

            Transform existingModel = currentModels.Find(m => m.name.Contains(m_ModelRef.name));
            if (existingModel)
            {
                existingModel.gameObject.SetActive(true);
            }
            else
            {
                Transform clone = Object.Instantiate(m_ModelRef, objectToVisualiseWeapon).transform;
                var transform = clone.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                clone.name = clone.name.Replace("(Clone)", "");
            }
        }
    }


    public static class WeaponFireType
    {
        public static void HitScan(Transform transform, float range, Action<DamageableObject> onFireHit)
        {
            Ray ray = new Ray(transform.position, transform.forward.normalized);
            Physics.Raycast(ray, out var hitInfo, range);
            onFireHit?.Invoke(hitInfo.collider.GetComponent<DamageableObject>());
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
}