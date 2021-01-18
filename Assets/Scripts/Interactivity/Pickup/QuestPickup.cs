using Player.Weapons;
using UnityEngine;

namespace Interactivity.Pickup
{
    public class QuestPickup : BasePickup
    {
        public override bool OnPickup(Weapon weapon = null)
        {
            return true;
        }
    }
}