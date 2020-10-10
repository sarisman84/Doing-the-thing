using System;
using UnityEngine.InputSystem;

namespace Extensions.InputExtension
{
    public static class InputReferenceExtension
    {
        public static void SetActive(this InputActionReference reference, bool value)
        {
            if (value)
            {
                reference.action.Enable();
                return;
            }
            
            reference.action.Disable();
           
        }


        public static T GetInputValue<T>(this InputActionReference reference) where T : struct
        {
            T v = default(T);
            if (v is bool)
            {
                float a = reference.action.ReadValue<float>();
                return (T) Convert.ChangeType(a == 1, typeof(T));
            }

            return reference.action.ReadValue<T>();
        }
        
        
    }

    public static class InputExtension
    {
        public static void SetActiveAll(bool value, params InputActionReference[] references)
        {
            for (int i = 0; i < references.Length; i++)
            {
                if(references[i] == null) continue;;
                references[i].SetActive(value);
                
            }
        }
    }
}