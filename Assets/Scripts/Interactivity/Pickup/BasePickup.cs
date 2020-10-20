using System;
using Player;
using UnityEngine;

namespace Interactivity.Pickup
{
    public abstract class BasePickup : MonoBehaviour
    {
        public abstract bool OnPickup(FirstPersonController controller);
        
    }

    
}