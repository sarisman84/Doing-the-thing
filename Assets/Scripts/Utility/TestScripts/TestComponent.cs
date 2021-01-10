using Interactivity.Events;
using UnityEngine;
using Utility.Attributes;

namespace Utility.TestScripts
{
    public class TestComponent : MonoBehaviour
    {
        [Expose] public ScriptableObject customEvent;
        [Expose] public ScriptableObject someOtherObject;
    }
}