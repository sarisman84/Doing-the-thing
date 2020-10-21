﻿using System;
using Extensions;
using Player;
using Spyro.Optimisation.ObjectManagement;
using UnityEngine;
using Random = System.Random;

namespace Interactivity.Pickup
{
    public abstract class BasePickup : MonoBehaviour
    {
        public abstract bool OnPickup(FirstPersonController controller);
        
        public static void SpawnCurrency(Transform owner, int minAmount, int maxAmount)
        {
            Random rnd = new Random();
            for (int i = 0; i < rnd.Next(minAmount, maxAmount); i++)
            {
                GameObject obj = ObjectManager.DynamicInstantiate(Resources.Load<GameObject>("Drops/Currency"));
                obj.SetActive(true);
                obj.transform.position = owner.position.GetRandomPositionInRange(2);
            }
        }

        public static void SpawnAmmo(Transform owner, string type, int amount)
        {
            BasePickup ammo = ObjectManager.DynamicComponentInstantiate(Resources.Load<BasePickup>($"Drops/{type}"));
            ammo.gameObject.SetActive(true);
            ammo.transform.position = owner.position;
        }
        
    }

    
}