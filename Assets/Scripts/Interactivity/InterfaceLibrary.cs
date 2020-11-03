using UnityEngine;

namespace Interactivity
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }


    public interface IPolymorphable
    {
        void Transform(GameObject newModel);

        bool HasTransformed { get; }
    }
}