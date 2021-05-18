using System;
using System.Collections.Generic;
using System.Linq;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts
{
    public static class WeaponFireType
    {
        public static RaycastHit HitScan(Transform transform, float range, Action<Transform, RaycastHit> beamVFX = null)
        {
            Ray ray = new Ray(transform.position, transform.forward.normalized);
            Physics.Raycast(ray, out var hitInfo, range);
            beamVFX?.Invoke(transform, hitInfo);
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

    public static class ProjectileMethods
    {
        public static Projectile IsAffectedByGravity(this Projectile projectile)
        {
            projectile.ONEnableEvent += (self) => self.physics.useGravity = true;
            return projectile;
        }


        public static Projectile Throw(this Projectile projectile, Transform barrel, float force)
        {
            projectile.IsAffectedByGravity();
            projectile.ONSinglePhysicsEvent += (self) =>
            {
                Vector3 direction = new Vector3(barrel.forward.x, barrel.forward.y / 2f, barrel.forward.z).normalized +
                                    Vector3.up * 0.5f;
                self.physics.AddForce(direction * (force * 10));
            };
            return projectile;
        }

        public static Projectile DestroyOnContact(this Projectile projectile, Transform transform)
        {
            projectile.ONCollisionEvent += (c, self) => DestroySelf(c, transform, self);
            return projectile;
        }

        public static Projectile Homing(this Projectile projectile, float detectionRadius, float turningSpeed = 2)
        {
            Transform closestTarget =
                projectile.transform.position.GetTheClosestEntityOfType<DamageableObject>(detectionRadius);
            projectile.ONFixedUpdateEvent += (self) =>
            {
                Vector3 direction = closestTarget
                    ? Vector3.Lerp(self.transform.forward.normalized,
                        (closestTarget.position - self.physics.position).normalized, turningSpeed / 10f)
                    : self.transform.forward.normalized;
                self.transform.rotation =
                    Quaternion.Lerp(self.transform.rotation,
                        Quaternion.LookRotation((direction), Vector3.up), turningSpeed / 10f);
            };
            return projectile;
        }

        public static Projectile AddForwardForce(this Projectile projectile, float force)
        {
            projectile.ONFixedUpdateEvent += self =>
            {
                self.physics.velocity = self.transform.forward * (force * 100 * Time.fixedDeltaTime);
            };

            return projectile;
        }


        private static void DestroySelf(Collision obj, Transform transform, Projectile projectile)
        {
            bool isPlayer = obj != null && obj.collider && obj.collider.name ==
                transform.parent.parent.parent.parent.GetChild(0).name;
            if (isPlayer) return;
            projectile.gameObject.SetActive(false, true);
            projectile.ResetAllEvents();
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
        private static LineRenderer beam;

        public static RaycastHit Beam(Transform barrel, RaycastHit hitInfo)
        {
            beam = beam ? beam : new GameObject("Beam").AddComponent<LineRenderer>();
            if (beam.transform.parent != barrel)
            {
                var transform = beam.transform;
                transform.parent = barrel;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        
            
            
            beam.SetPosition(1, Vector3.forward * hitInfo.distance);
       
            
            return hitInfo;
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