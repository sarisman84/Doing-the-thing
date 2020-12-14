using System;
using Interactivity.Components;
using UnityEngine;

namespace Interactivity.Events
{
    [CreateAssetMenu(fileName = "New Damage Event", menuName = "Event/Damage Event", order = 0)]
    public class DamageEvent : CustomEvent
    {
        [Tooltip(
            "Dictates when the event will be called by checking if the assigned entity has more health than the assigned limit.")]
        public float healthLimit;

        public CustomEvent whileUnderHealthLimit;

        private DamageableEntity _entity;

        private void OnEnable()
        {
            _entity = null;
        }

        public void OnInvokeEvent(Collider col)
        {
            DamageableEntity foundEntity = col.GetComponent<DamageableEntity>();
            _entity = _entity == foundEntity || !foundEntity ? _entity : foundEntity;

            if (_entity)
                OnInvokeEvent(_entity.CurrentHealth);

            IsBeingCalled = false;
        }


        public void OnInvokeEvent(int currentHealth)
        {
            if (currentHealth > healthLimit)
            {
                OnInvokeEvent();
                return;
            }

            IsBeingCalled = false;
            if (whileUnderHealthLimit)
                whileUnderHealthLimit.OnInvokeEvent();
        }
    }
}