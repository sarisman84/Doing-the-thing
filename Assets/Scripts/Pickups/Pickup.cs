using Player;
using UnityEngine;

namespace Scripts
{
    public abstract class Pickup : MonoBehaviour
    {
        public abstract void OnPickup(PickupInteractor interactor);
    }

   
}