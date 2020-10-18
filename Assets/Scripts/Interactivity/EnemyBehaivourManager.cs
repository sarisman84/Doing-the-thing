using UnityEngine;

namespace Interactivity
{
    public class EnemyManager
    {
        private static EnemyManager _ins;
        
        public static EnemyManager Access
        {
            get
            {
                _ins = _ins == null ? _ins : new EnemyManager();
                return _ins;
            }
        }

        private Transform _target;

        public void AssignTarget(Transform target)
        {
            _target = target != null ? target : _target;
        }

        public Vector3 GetTargetPosition()
        {
            return _target == null ? Vector3.zero : _target.position;
        }


        public void ClearTarget()
        {
            _target = null;
        }
    }
}