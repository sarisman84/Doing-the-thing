using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Extensions;
using Extensions.InputExtension;
using Interactivity;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player.Weapons.VisualWeaponBehaivourLibrary;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Player.Weapons
{
    public static class WeaponManager
    {
        public static Dictionary<string, Weapon> globalWeaponLibrary = new Dictionary<string, Weapon>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameStart()
        {
            globalWeaponLibrary.Add("Test_Pistol",
                new Weapon(WeaponBehaviorLibrary.NormalFire, 500, 0.1f, 10, "Test_Pistol"));
            globalWeaponLibrary.Add("Eliott's Seal Generator",
                new Weapon(WeaponBehaviorLibrary.PolymorphRounds, 2500, 0.01f, 0, "Eliott's Seal Generator"));
            globalWeaponLibrary.Add("Rocket Launcher",
                new Weapon(WeaponBehaviorLibrary.ProjectileFire, 50, 1f, 500, "Rocket Launcher"));
        }
    }

    [Serializable]
    public class Weapon
    {
        public Weapon(Action<Weapon, Ray> fireEvent, int maxAmmo, float fireRate, float damage, string weaponName)
        {
            this.fireEvent = fireEvent;
            this.fireRate = fireRate;

            _localAmmoCount = maxAmmo;
            maxAmmoCount = maxAmmo;

            _localCounter = fireRate;

            this.damage = damage;

            model = Object.Instantiate(Resources.Load<GameObject>($"WeaponModels/{weaponName}_Model"));
            model.SetActive(false);
            icon = Resources.Load<Sprite>($"WeaponIcons/{weaponName}_Icon");


            name = weaponName;

            _id = Guid.NewGuid();
        }

        public Transform WeaponBarrel
        {
            get => model.transform.GetChild(0);
        }

        public Action<Weapon, Ray> fireEvent;
        public float fireRate;
        public float damage;
        public int maxAmmoCount;

        public string name;
        public GameObject model;
        public Sprite icon;

        private Guid _id;

        public Guid ID
        {
            get => _id;
        }

        public int currentAmmoCount => _localAmmoCount;

        private float _localCounter = 0;
        private int _localAmmoCount = 0;

        public void OnWeaponPrimaryFire(InputActionReference primaryFire, WeaponController controller)
        {
            _controller = _controller ?? controller;
            _localCounter += Time.deltaTime;
            _localCounter = Mathf.Clamp(_localCounter, 0, fireRate);

            var transform = controller.player.playerCamera.transform;
            var firePosition = transform.position;
            var fireDirection = transform.forward;

            Ray weaponRay = new Ray(firePosition, fireDirection);


            if (_localCounter.Equals(fireRate) && primaryFire.GetInputValue<bool>() && _localAmmoCount > 0)
            {
                fireEvent.Invoke(this, weaponRay);
                _controller.hudManager.UpdateAmmoCounter(this);
                _localCounter = 0;
                _localAmmoCount--;
            }
        }

        private WeaponController _controller;

        public bool AddAmmo(int i)
        {
            if (_localAmmoCount >= maxAmmoCount) return false;
            _localAmmoCount += maxAmmoCount;
            _localAmmoCount = Mathf.Clamp(_localAmmoCount, 0, maxAmmoCount);
            _controller.hudManager.UpdateAmmoCounter(this);
            return true;
        }
    }


    public static class WeaponBehaviorLibrary
    {
        public static void NormalFire(Weapon weapon, Ray trajectoryInformation)
        {
            RaycastHit closestHit = Physics.RaycastAll(trajectoryInformation).GetClosestHit();

            DamageEntity(closestHit.collider, weapon.damage);

            TracerRounds(weapon.model.transform, weapon.WeaponBarrel.transform, closestHit.point,
                new Color(1, 0.56f, 0.25f, 1));
        }

        private static void DamageEntity(Collider hit, float damage)
        {
            if (hit)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }
        }

        private static void TransformEntity(this Weapon weapon, RaycastHit hit, string transformation)
        {
            if (hit.collider)
            {
                IPolymorphable polymorphable = hit.collider.GetComponent<IPolymorphable>();
                polymorphable?.Transform(Resources.Load<GameObject>($"Enemies/{transformation}"));
            }
        }


        public static void PolymorphRounds(Weapon weapon, Ray trajectoryInformation)
        {
            RaycastHit closestHit = Physics.RaycastAll(trajectoryInformation).GetClosestHit();

            weapon.TransformEntity(closestHit, "Seal");

            TracerRounds(weapon.model.transform, weapon.WeaponBarrel.transform, closestHit.point, Color.cyan);
        }

        public static void ProjectileFire(Weapon weapon, Ray trajectoryInformation)
        {
            Projectile projectile = new Projectile(weapon.WeaponBarrel.transform.position,
                Quaternion.LookRotation(trajectoryInformation.direction, Vector3.up));
            projectile.Physics.useGravity = false;
            projectile.Physics.constraints = RigidbodyConstraints.FreezeRotation;
            projectile.Physics.AddForce(trajectoryInformation.direction * 1000f, ForceMode.Force);
            projectile.FetchInformation(weapon.damage, 5f, Explosion);
            projectile.SetProjectileModel("Fireball");
        }

        private static void Explosion(Transform transform, float radius, float damage)
        {
            List<Collider> foundObjects = Physics.OverlapSphere(transform.position, radius * 2f).ToList();

            var orderedEnumerable =
                foundObjects.OrderByDescending(c => Vector3.Distance(transform.position, c.transform.position));


            foreach (var entity in orderedEnumerable)
            {
                if (Vector3.Distance(transform.position, entity.transform.position) <= radius &&
                    !entity.GetComponent<FirstPersonController>())
                    DamageEntity(entity, damage);
                KnockbackEntity(entity, transform, radius);
            }

            transform.gameObject.SetActive(false);
        }

        private static void KnockbackEntity(Collider entity, Transform transform, float radius)
        {
            Rigidbody rigidbody = entity.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.AddExplosionForce(1050f, transform.position, radius, 2f);
            }
        }
    }


    public static class VisualWeaponBehaivourLibrary
    {
        private static TrailRenderer _traceRenderer;

        public static void TracerRounds(Transform parent, Transform origin, Vector3 targetPoint, Color color)
        {
            if (_traceRenderer)
                _traceRenderer.Clear();

            _traceRenderer = _traceRenderer
                ? _traceRenderer
                : Object.Instantiate(Resources.Load<TrailRenderer>("WeaponEffects/Bullet Trail"));
        
            Transform transform;
            (transform = _traceRenderer.transform).SetParent(parent);
            transform.position = origin.position;

            var material = _traceRenderer.material;
            _traceRenderer.material = material ? material : Resources.Load<Material>("Materials/Default_Line");
            _traceRenderer.AddPosition(targetPoint.TryGetValue(origin));
            _traceRenderer.startColor = color;
            _traceRenderer.endColor = color;
            
        }
    }
}