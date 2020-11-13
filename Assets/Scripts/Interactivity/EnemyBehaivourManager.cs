using System;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using Utility;

namespace Interactivity
{
    public class EnemyBehaivourManager
    {
        private const string c_AssignNewTarget = "Enemy_AssignNewTarget";
        private const string c_GetTargetPosition = "Enemy_TrackTarget";
        private const string c_RemoveCurrentTarget = "Enemy_ClearTarget";


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameLoad()
        {
            EnemyBehaivourManager manager = new EnemyBehaivourManager();
            EventManager.AddListener(c_AssignNewTarget,
                new Action<Transform>(value => manager.AssignTarget(value)));
            EventManager.AddListener(c_GetTargetPosition,
                new Func<Vector3>(() => manager._GetTargetPosition()));
            EventManager.AddListener(c_RemoveCurrentTarget, new Action(() => { manager.ClearTarget(); }));
        }


        public static void AssignNewTarget(Transform value)
        {
            EventManager.TriggerEvent(c_AssignNewTarget, value);
        }

        public static Vector3 GetCurrentTargetPosition()
        {
            return (Vector3)EventManager.TriggerEvent(c_GetTargetPosition);
        }

        public static void ClearTargetFocus()
        {
            EventManager.TriggerEvent(c_RemoveCurrentTarget);
        }


        private Transform _target;

        private void AssignTarget(Transform target)
        {
            if (target)
            {
                _target = target;
            }
        }

        private Vector3 _GetTargetPosition()
        {
            return _target.Equals(null) ? Vector3.zero : _target.position;
        }


        private void ClearTarget()
        {
            _target = null;
        }
    }
}