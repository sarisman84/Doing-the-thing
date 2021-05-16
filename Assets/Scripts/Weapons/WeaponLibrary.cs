using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        (onContact, self) => onContact.contacts[0].point.Explosion(10f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(5)))
                    .Throw(o, 1000).DestroyOnContact(o));

            AddWeaponToLibrary("default_rocket_launcher", "Rocket Launcher", 0.15f,
                o => WeaponFireType.Projectile(o, "default_rocket_launcher_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(15)))
                    .Homing(3.5f, 50f).DestroyOnContact(o).DestroyAfterSeconds(o, 5f));
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
        private float m_FireRate;
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

        public static Projectile Projectile(Transform transform, string projectileId,
            Action<Collision, Projectile> onContactEvent)
        {
            //Spawn projectile at transform's position and with transform's rotation (forward)
            //Add some events to it.
            Projectile projectile = Utility.DynamicInstantiate(
                    Resources.Load<GameObject>($"Weapons/Projectiles/Model Prefabs/{projectileId}"),
                    transform.position, transform.rotation)
                .AddComponent<Projectile>();
            projectile.ONCollisionEvent += onContactEvent;

            return projectile;
        }
    }

    public static class ProjectileEffects
    {
        public static Projectile Throw(this Projectile projectile, Transform barrel, float force)
        {
            bool hasAlreadyUpdated = false;
            projectile.ONFixedUpdateEvent += (self) =>
            {
                if (!hasAlreadyUpdated)
                {
                    self.physics.AddForce(barrel.forward.normalized * force);
                    hasAlreadyUpdated = true;
                }
            };
            return projectile;
        }

        public static Projectile DestroyOnContact(this Projectile projectile, Transform transform)
        {
            projectile.ONCollisionEvent += (c, self) => DestroySelf(c, transform, self);
            return projectile;
        }

        public static Projectile Homing(this Projectile projectile, float force, float radius)
        {
            Transform closestTarget = projectile.transform.position.GetTheClosestEntityOfType<DamageableObject>(radius);
            projectile.ONFixedUpdateEvent += (self) =>
            {
                Vector3 direction = closestTarget
                    ? Vector3.Lerp(self.transform.forward.normalized, (closestTarget.position - self.physics.position).normalized, 0.2f)
                    : self.transform.forward.normalized;
                float trueForce = (force * 100 * Time.fixedDeltaTime);
                self.physics.velocity = direction * trueForce;

                self.physics.useGravity = false;
                self.transform.rotation =
                    Quaternion.Lerp(self.transform.rotation,
                        Quaternion.LookRotation((direction), Vector3.up), 0.2f);
            };
            return projectile;
        }


        private static void DestroySelf(Collision obj, Transform transform, Projectile projectile)
        {
            bool isPlayer = obj != null && obj.collider && obj.collider.name ==
                            transform.parent.parent.parent.parent.GetChild(0).name;
            if (isPlayer) return;
            projectile.gameObject.SetActive(false, true);
            projectile.ResetCollisionEvent();
            projectile.ResetUpdateEvent();
            projectile.ResetFixedUpdateEvent();
        }

        public static Projectile DestroyAfterSeconds(this Projectile projectile, Transform barrel, float seconds)
        {
            float countdown = 0;
            projectile.ONUpdateEvent += (self) =>
            {
                countdown += Time.deltaTime;
                if (countdown >= seconds)
                {
                    DestroySelf(null, barrel, self);
                    countdown = 0;
                }
            };
            return projectile;
        }
    }

    public static class WeaponHitType
    {
        public static Collider DealDamage(this Collider entity, int damage)
        {
            if (entity && entity.GetComponent<DamageableObject>() is { } damageableObject)
                damageableObject.TakeDamage(damage);
            return entity;
        }

        public static Collider[] Explosion(this Vector3 center, float radius)
        {
            return Physics.OverlapBox(center, Vector3.one * radius);
        }
    }

    public static class VisualEffects
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
            return DynamicInstantiate(gameObject.gameObject).GetComponent<T>();
        }


        public static GameObject DynamicInstantiate(GameObject gameObject)
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

            inactiveObj.SetActive(true);
            return inactiveObj;
        }

        public static GameObject DynamicInstantiate(GameObject gameObject, Vector3 position, Quaternion rotiation)
        {
            GameObject result = DynamicInstantiate(gameObject);
            result.transform.position = position;
            result.transform.rotation = rotiation;
            result.SetActive(true);
            return result;
        }

        public static void SetActive(this GameObject gameObject, bool activeState, bool resetPhysics = false)
        {
            Rigidbody physics = gameObject.GetComponent<Rigidbody>();
            if (resetPhysics && physics)
            {
                physics.velocity = Vector3.zero;
                physics.angularVelocity = Vector3.zero;
            }

            gameObject.SetActive(activeState);
        }

        #endregion
    }
}