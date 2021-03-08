// using System;
// using System.Collections.Generic;
// using Extensions;
// using Spyro.Optimisation.ObjectManagement;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Player.Weapons
// {
//     public class Projectile
//     {
//         private static GameObject _blueprintProjectile;
//
//         [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//         public static void OnGameStart()
//         {
//             new GameObject("Projectile").Execute(g =>
//             {
//                 g.AddComponent<Rigidbody>();
//                 g.AddComponent<SphereCollider>();
//                 g.AddComponent<ProjectileBehaivour>();
//                 _blueprintProjectile = g;
//
//                 ObjectManager.PoolGameObject(_blueprintProjectile, 500);
//                 _blueprintProjectile.SetActive(false);
//             });
//         }
//
//         public GameObject GameObject { get; }
//         public SphereCollider Collider { get; }
//         ProjectileBehaivour MonoBehaivour { get; }
//         public Rigidbody Physics { get; }
//
//
//         Dictionary<string, GameObject> _knownModels = new Dictionary<string, GameObject>();
//
//
//         public Projectile(Vector3 position, Quaternion rotation)
//         {
//             GameObject = ObjectManager.DynamicInstantiate(_blueprintProjectile);
//             GameObject.SetActive(true);
//             Physics = GameObject.GetComponent<Rigidbody>();
//             Collider = GameObject.GetComponent<SphereCollider>();
//             MonoBehaivour = GameObject.GetComponent<ProjectileBehaivour>();
//
//             Collider.ChangeSize(0.15f);
//
//             Physics.useGravity = true;
//             Physics.velocity = Vector3.zero;
//
//             GameObject.transform.position = position;
//             GameObject.transform.rotation = rotation;
//         }
//
//         public void SetProjectileModel(string model)
//         {
//             _knownModels.ApplyAction(t =>
//             {
//                 if (!t.Value.Equals(null)) t.Value.SetActive(false);
//             });
//             if (_knownModels.ContainsKey(model))
//             {
//                 _knownModels[model].SetActive(true);
//             }
//             else
//             {
//                 GameObject obj = Object.Instantiate(Resources.Load<GameObject>($"Projectiles/{model}_Projectile"),
//                     GameObject.transform);
//                 _knownModels.Add(model, obj);
//             }
//         }
//
//         public void FetchInformation(float weaponDamage, float hitRadius, OnCollisionEnterEvent damageEntity)
//         {
//             MonoBehaivour.CollisionRadious = hitRadius;
//             MonoBehaivour.ProjectileDamage = weaponDamage;
//             MonoBehaivour.OnHitEvent += damageEntity;
//         }
//     }
// }