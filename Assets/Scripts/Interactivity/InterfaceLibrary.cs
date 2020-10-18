using UnityEngine;

namespace Interactivity
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }


    public interface IPathfind
    {
        Vector3 PathfindToTarget(Vector3 target);
    }
}