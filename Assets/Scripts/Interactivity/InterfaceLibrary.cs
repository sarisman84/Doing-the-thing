using Player;
using UnityEngine;

namespace Interactivity
{
    public interface IDamageable : IUnity
    {
        void TakeDamage(float damage);
        bool IsDead { get; }
        float CurrentHealth { get; set; }
    }


    public interface IPolymorphable : IUnity
    {
        void Transform(GameObject newModel);

        bool HasTransformed { get; }
    }

    public enum InteractionInput
    {
        Hold,
        Press
    }

    public interface IInteractable : IUnity
    {
        void OnInteract(PlayerController controller);
        void OnProximityEnter();
        void OnProximityExit();

        InteractionInput InputType { get; }
        bool NeedToLookAtInteractable { get; }
    }

    public interface IUnity
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}