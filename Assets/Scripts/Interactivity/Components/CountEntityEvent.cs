using Extensions;
using Interactivity.Events;
using UnityEngine;

namespace Interactivity.Components
{
    [CreateAssetMenu(fileName = "New Count Condition", menuName = "Event/Count Condition", order = 0)]
    public class CountEntityEvent : CustomEvent
    {
        private int _count;

        private int CurrentCount
        {
            get => _count;
            set
            {
                _count = value;
                _count = Mathf.Clamp(_count, 0, maxCount);
            }
        }

        public GameObject entityToCount;
        public int maxCount;


        protected override void OnEnable()
        {
            CurrentCount = 0;
            hasTriggered = false;
        }

        public object CountEntity(GameObject detectedEntity,
            params object[] args)
        {
            if (detectedEntity)
            {
                if (detectedEntity.name.Contains(entityToCount.name))
                {
                    CurrentCount++;
                }
            }

            return Raise(true, args);
        }

        public override object Raise(bool triggerEventOnce, object[] args)
        {
            if (CurrentCount == maxCount && !hasTriggered)
            {
                hasTriggered = triggerEventOnce;
                return gameEvent?.Invoke(args);
            }

            return default;
        }
    }
}