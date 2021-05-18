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
            AddWeaponToLibrary("default_gun", "default_gun_icon", "Blaster", 150, 25, 0.25f,
                o => WeaponFireType.HitScan(o, 15f).collider.DealDamage(10));

            AddWeaponToLibrary("default_grenade", "default_grenade_icon", "Grenade", 40, 15, 0.75f,
                o => WeaponFireType.Projectile(o, "default_grenade_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(5)))
                    .Throw(o, 50).DestroyOnContact(o));

            AddWeaponToLibrary("default_rocket_launcher", "default_rocket_launcher_icon", "Rocket Launcher", 20, 5,
                0.15f,
                o => WeaponFireType.Projectile(o, "default_rocket_launcher_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(15))).AddForwardForce(3.5f)
                    .Homing(50f).DestroyOnContact(o).DestroyAfterSeconds(o, 5f));
        }


        private static void AddWeaponToLibrary(string id, string icon_id, string name, int maxAmmo, int ammoPickupAmm,
            float fireRate,
            Action<Transform> weaponFireEvent)
        {
            GlobalWeaponLibrary.Add(id,
                new Weapon(name, id, string.IsNullOrEmpty(icon_id) ? "default_gun_icon" : icon_id, maxAmmo,
                    ammoPickupAmm, fireRate,
                    weaponFireEvent, Resources.Load<GameObject>($"Weapons/Model Prefabs/{id}")));
        }
    }


    public static class PickupManager
    {
        private static readonly List<Pickup> CurrentPickups = new List<Pickup>();

        public static void RegisterPickup(Pickup pickup)
        {
            if (CurrentPickups.Contains(pickup)) return;
            CurrentPickups.Add(pickup);
        }

        public static Pickup FetchClosestPickup(Vector3 position, float minDistance)
        {
            Pickup result = CurrentPickups.Find(d =>
                Vector3.Distance(d.transform.position, position) <= minDistance && d.isActiveAndEnabled);
            return result;
        }

        public static void SetActive(this Pickup pickup, bool value)
        {
            if (CurrentPickups.Contains(pickup))
            {
                CurrentPickups.Remove(pickup);
            }

            pickup.gameObject.SetActive(value);
        }
    }
}