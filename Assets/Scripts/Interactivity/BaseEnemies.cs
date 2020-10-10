using UnityEngine;

namespace Interactivity
{
    public class BaseEnemies : MonoBehaviour, IDamageable
    {
        public virtual void TakeDamage(float damage)
        {
            Debug.Log($"{name} took damage! Damage:{damage}");
        }
    }
}