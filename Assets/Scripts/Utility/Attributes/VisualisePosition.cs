using UnityEngine;

namespace Utility.Attributes
{
    public class VisualisePosition : PropertyAttribute
    {
        public bool CanMoveHandle;
        public bool CanShowVariable;
        public bool IsPositionLocalized;

        public VisualisePosition(bool isPositionLocal, bool moveHandle = true, bool showVariableInScene = false)
        {
            CanMoveHandle = moveHandle;
            CanShowVariable = showVariableInScene;
            IsPositionLocalized = isPositionLocal;
        }
    }
}