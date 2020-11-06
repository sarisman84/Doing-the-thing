using UnityEngine;

namespace Interactivity
{
    public interface IDamageable: IUnity
    {
        void TakeDamage(float damage);
    }


    public interface IPolymorphable: IUnity
    {
        void Transform(GameObject newModel);

        bool HasTransformed { get; }
    }

    public interface IInteractable: IUnity
    {
        void OnInteract();
    }

    public interface IUnity
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}