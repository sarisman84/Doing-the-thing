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
            AddWeaponToLibrary("default_gun", "default_gun_icon","Blaster", 150,0.25f,
                o => WeaponFireType.HitScan(o, 15f).collider.DealDamage(10));

            AddWeaponToLibrary("default_grenade","default_grenade_icon" ,"Grenade", 40,0.75f,
                o => WeaponFireType.Projectile(o, "default_grenade_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(5)))
                    .Throw(o, 50).DestroyOnContact(o));

            AddWeaponToLibrary("default_rocket_launcher", "default_rocket_launcher_icon", "Rocket Launcher", 20, 0.15f,
                o => WeaponFireType.Projectile(o, "default_rocket_launcher_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(15))).AddForwardForce(3.5f)
                    .Homing(50f).DestroyOnContact(o).DestroyAfterSeconds(o, 5f));
        }


        private static void AddWeaponToLibrary(string id, string icon_id, string name, int maxAmmo, float fireRate,
            Action<Transform> weaponFireEvent)
        {
            GlobalWeaponLibrary.Add(id,
                new Weapon(name, string.IsNullOrEmpty(icon_id) ? "default_gun_icon" : icon_id, maxAmmo, fireRate,
                    weaponFireEvent, Resources.Load<GameObject>($"Weapons/Model Prefabs/{id}")));
        }
    }
}