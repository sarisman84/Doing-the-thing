using UnityEngine;

namespace Interactivity.Events
{
    public class DebugTest : MonoBehaviour
    {
        public void Print(string str)
        {
            Debug.Log(str);
        }

        public void Print(int value)
        {
            
            Print($" Health Detected {value}");
        }
    }
}
