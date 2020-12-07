using System;
using Player;
using UnityEngine;

namespace Interactivity
{
    public interface IDamageable : IUnity
    {
        void TakeDamage(Collider col, float damage);
        void OnDeath(Collider col);
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
        void OnInteract(Collider collider);
        IInteractable AddCallback(Action<Collider> callback);
        InteractionInput InputType { get; }
     
    }

    public interface IDetectable : IUnity
    {
        void OnAreaEnter(Collider col);
        void OnAreaStay(Collider collider);
        void OnAreaExit(Collider collider);
        
    }

  

    public interface IUnity
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}