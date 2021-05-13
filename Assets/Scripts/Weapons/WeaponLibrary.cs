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
            AddWeaponToLibrary("default_gun", "Blaster", 0.25f,
                o => WeaponFireType.HitScan(o, 15f).collider.DealDamage(10));
            AddWeaponToLibrary("default_grenade", "Grenade", 0.75f,
                o => WeaponFireType.Projectile(o, "default_grenade_projectile",
                        onContact => onContact.contacts[0].point.Explosion(10f).ForEach(o => o.DealDamage(5)))
                    .Throw(o, 1000));
        }


        private static void AddWeaponToLibrary(string id, string name, float fireRate,
            Action<Transform> weaponFireEvent)
        {
            GlobalWeaponLibrary.Add(id,
                new Weapon(name, fireRate, weaponFireEvent, Resources.Load<GameObject>($"Weapons/Model Prefabs/{id}")));
        }
    }

    [Serializable]
    public class Weapon
    {
        private string m_WeaponName;
        private float m_FireRate = 0;
        private float m_CurrentRate;
        public Action<Transform> onWeaponFire;

        private Transform m_Model;

        public Weapon(string name, float fireRate, Action<Transform> weaponFire, GameObject model)
        {
            m_WeaponName = name;
            m_FireRate = fireRate;
            onWeaponFire = weaponFire;

            Debug.Log($"Init: Added Weapon {m_WeaponName} to Global Library.");

            try
            {
                Transform clone = Object.Instantiate(model).transform;
                var transform = clone.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                clone.name = clone.name.Replace("(Clone)", "");
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
            if (m_CurrentRate >= m_FireRate && trigger)
            {
                onWeaponFire?.Invoke(m_Model.GetChildWithTag("Weapon/BarrelPoint"));
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
        public static RaycastHit HitScan(Transform transform, float range)
        {
            Ray ray = new Ray(transform.position, transform.forward.normalized);
            Physics.Raycast(ray, out var hitInfo, range);
            return hitInfo;
        }

        public static Projectile Projectile(Transform transform, string projectileId, Action<Collision> onContactEvent)
        {
            //Spawn projectile at transform's position and with transform's rotation (forward)
            //Add some events to it.
            Projectile projectile = Object
                .Instantiate(Resources.Load<GameObject>($"Weapons/Projectiles/Model Prefabs/{projectileId}"),
                    transform.position, transform.rotation)
                .AddComponent<Projectile>();
            projectile.ONCollisionEvent += onContactEvent;
            projectile.ONCollisionEvent += c => DestroySelf(c, projectile);
            return projectile;
        }

        private static void DestroySelf(Collision obj, Projectile projectile)
        {
            //Object.Destroy(projectile.gameObject);
        }
    }

    public static class ProjectileEffects
    {
        public static Projectile Throw(this Projectile projectile, Transform barrel, float force)
        {
            bool hasAlreadyUpdated = false;
            projectile.ONFixedUpdateEvent += () =>
            {
                if (!hasAlreadyUpdated)
                {
                    projectile.physics.AddForce(barrel.forward.normalized * force);
                    hasAlreadyUpdated = true;
                }
            };
            return projectile;
        }

        public static Projectile Homing(this Projectile projectile)
        {
            return projectile;
        }
    }

    public static class WeaponHitType
    {
        public static Collider DealDamage(this Collider raycastHit, int damage)
        {
            if (raycastHit && raycastHit.GetComponent<DamageableObject>() is { } damageableObject)
                damageableObject.TakeDamage(damage);
            return raycastHit;
        }

        public static Collider[] Explosion(this Vector3 center, float radius)
        {
            return Physics.OverlapBox(center, Vector3.one * radius);
        }
    }

    public static class WeaponFireEffectType
    {
        public static RaycastHit Beam(this RaycastHit ray, Color beamColor)
        {
            return ray;
        }
    }


    public static class Utility
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> elements, Action<T> actionEvent)
        {
            var enumerable = elements as T[] ?? elements.ToArray();
            foreach (var element in enumerable)
            {
                actionEvent.Invoke(element);
            }

            return enumerable;
        }


        #region Object Pooling

        public static Dictionary<int, List<GameObject>> DictionaryOfPooledGameObjects =
            new Dictionary<int, List<GameObject>>();


        private static void PoolGameObjects(GameObject original, int amount)
        {
            bool alreadyExistsAList = DictionaryOfPooledGameObjects.ContainsKey(original.GetInstanceID());
            Transform parent = alreadyExistsAList
                ? DictionaryOfPooledGameObjects[original.GetInstanceID()][0].transform.parent
                : new GameObject($"{original.name}:{amount}").transform;

            List<GameObject> pooledGameObjects = new List<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                GameObject clone = Object.Instantiate(original, parent);
                clone.name.Replace("Clone", $"{i + 1}");
                clone.SetActive(false);
                pooledGameObjects.Add(clone);
            }


            if (alreadyExistsAList)
            {
                DictionaryOfPooledGameObjects[original.GetInstanceID()].AddRange(pooledGameObjects);
            }
            else
            {
                DictionaryOfPooledGameObjects.Add(original.GetInstanceID(), pooledGameObjects);
            }
        }

        public static T DynamicInstantiate<T>(T gameObject) where T : MonoBehaviour
        {
            List<GameObject> obj = DictionaryOfPooledGameObjects
                .FirstOrDefault(g => g.Key == gameObject.GetInstanceID()).Value;
            if (obj == null)
            {
                PoolGameObjects(gameObject.gameObject, 500);
                return DynamicInstantiate(gameObject);
            }
            GameObject inactiveObj = obj.Find(g => !g.activeSelf);
            if (!inactiveObj)
            {
                PoolGameObjects(gameObject.gameObject, 500);
                return DynamicInstantiate(gameObject);
            }
            return inactiveObj.GetComponent<T>();
        }

        #endregion
    }
}