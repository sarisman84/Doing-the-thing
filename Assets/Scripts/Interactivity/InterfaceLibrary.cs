using System;
using Player;
using UnityEngine;

namespace Interactivity
{
    public interface IDamageable : IUnity
    {
    
        void TakeDamage(GameObject attacker, float damage);
        void OnDeath(GameObject attacker);
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
        Collider LatestInteractor { get; }
        void OnInteract(Collider collider);
        void OnHoverEnter(Collider collider);
        void OnHoverStay(Collider collider);
        void OnHoverExit(Collider collider);
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