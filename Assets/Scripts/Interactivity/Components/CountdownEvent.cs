using System;
using System.Collections;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components
{
    [CreateAssetMenu(fileName = "New Time Condition", menuName = "Event/Time Condition", order = 0)]
    public class TimedCondition : CustomEvent
    {
        private float _startingTime;

        private void OnEnable()
        {
            _startingTime = 0;
        }

        public override object Raise(object[] args)
        {
            int i = 0;
            if (args[0] is int targetTime)
                i = targetTime;
            if (_startingTime == 0) ResetCount();
            else if (Time.time - _startingTime >= i)
            {
                object[] newArgs = new object[args.Length - 1];
                Array.Copy(args, 1, newArgs, 0, newArgs.Length);
                return gameEvent?.Invoke(newArgs);
            }

            return default;
        }

        private IEnumerator DelayRaise()
        {
            throw new NotImplementedException();
        }


        public void ResetCount()
        {
            _startingTime = Time.time;
        }
    }
}