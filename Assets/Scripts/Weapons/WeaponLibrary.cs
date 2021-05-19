using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using General_Scripts.Utility.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Weapons
{
    public static class WeaponLibrary
    {
        public static readonly Dictionary<string, Weapon> GlobalWeaponLibrary = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnGameStart()
        {
            AddWeaponToLibrary("default_gun", "Blaster", 150, 25, 0.25f,
                o => WeaponFireType.HitScan(o, 15f).collider.DealDamage(10));

            AddWeaponToLibrary("default_grenade", "Grenade", 40, 15, 0.75f,
                o => WeaponFireType.Projectile(o, "default_grenade_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(5)))
                    .Throw(o, 50).DestroyOnContact(o));

            AddWeaponToLibrary("default_rocket_launcher", "Rocket Launcher", 20, 5,
                0.15f,
                o => WeaponFireType.Projectile(o, "default_rocket_launcher_projectile",
                        (onContact, self) => onContact.contacts[0].point.Explosion(2.5f)
                            .ForEach(entityInExplosion => entityInExplosion.DealDamage(15))).AddForwardForce(3.5f)
                    .HomeTowardsEnemies(50f).DestroyOnContact(o).DestroyAfterSeconds(o, 5f));
        }


        public static Weapon AddWeaponToLibrary(string id, string name, int maxAmmo, int ammoPickupAmm,
            float fireRate,
            Action<Transform> weaponFireEvent)
        {
            GlobalWeaponLibrary.Add(id,
                new Weapon(name, id, maxAmmo,
                    ammoPickupAmm, fireRate,
                    weaponFireEvent, Resources.Load<GameObject>($"Weapons/Model Prefabs/{id}")));

            return GlobalWeaponLibrary[id];
        }

        public static Weapon AddWeaponToLibrary(Weapon weapon)
        {
            GlobalWeaponLibrary.Add(weapon.ID, weapon);
            return weapon;
        }
    }
}