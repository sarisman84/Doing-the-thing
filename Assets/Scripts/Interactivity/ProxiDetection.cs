using System;
using UnityEngine;

namespace Interactivity
{
    public class ProxiDetectionSystem
    {
        public static void DetectEntityInProximity(Vector3 origin, Vector3 target, int range,
            ref float previousDistance, Action onEnter, Action onStay,
            Action onExit, bool onEnterCondition = true, bool onStayCondition = true, bool onExitCondition = true)
        {
            float distance = Mathf.Abs(Vector3.Distance(origin, target));

            if (distance >= previousDistance && onExitCondition)
            {
                previousDistance = 0;
                onExit?.Invoke();
            }
            else if (distance > range)
            {
                if (onStayCondition)
                    onStay?.Invoke();
                if (previousDistance == 0 && onEnterCondition)
                {
                    previousDistance = distance;
                    onEnter?.Invoke();
                }
            }
        }

        public static void DetectEntityOnEnteringProximity(Vector3 origin, Vector3 target, int range,
            ref float previousDistance, Action onEnter, bool onEnterCondition = true)
        {
            DetectEntityInProximity(origin, target, range, ref previousDistance, onEnter, null, null, onEnterCondition);
        }


        public static void DetectEntityOnExitingProximity(Vector3 origin, Vector3 target, int range,
            ref float previousDistance, Action onExit, bool onExitCondition = true)
        {
            DetectEntityInProximity(origin, target, range, ref previousDistance, null, null, onExit, true,
                false, onExitCondition);
        }

        public static void DetectEntityWhileStayingInProximity(Vector3 origin, Vector3 target, int range,
            ref float previousDistance, Action onStay, bool onStayCondition = true)
        {
            DetectEntityInProximity(origin, target, range, ref previousDistance, null, onStay, null, true,
                onStayCondition , true);
        }
    }
}