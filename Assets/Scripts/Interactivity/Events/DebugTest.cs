using System;
using UnityEngine;

namespace Interactivity.Events
{
    public class DebugTest : MonoBehaviour
    {
        private Color _originalColor;
        private MeshRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer)
                _originalColor = _renderer.material.color;
        }

        public void Print(string str)
        {
            Debug.Log(str);
        }

        public void Print(int value)
        {
            Print($" Health Detected {value}");
        }


        public void AlterColor(Color newColor)
        {
            _renderer.sharedMaterial.color = newColor;
        }

        public void ResetColor()
        {
            _renderer.sharedMaterial.color = _originalColor;
        }
    }
}