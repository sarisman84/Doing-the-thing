using UnityEngine;

namespace Interactivity
{
    public class EnemyBehaivourManager
    {
        private static EnemyBehaivourManager _ins;
        
        public static EnemyBehaivourManager Access
        {
            get
            {
                _ins = _ins ?? new EnemyBehaivourManager();
                return _ins;
            }
        }

        private Transform _target;

        public void AssignTarget(Transform target)
        {
            if (target)
            {
                _target = target;
            }
        }

        public Vector3 GetTargetPosition()
        {
            return _target.Equals(null) ? Vector3.zero : _target.position;
        }


        public void ClearTarget()
        {
            _target = null;
        }
    }
}