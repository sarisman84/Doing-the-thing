using System;
using Extensions;
using UnityEngine;

namespace Interactivity.Vendor
{
    public class OutlineManager : MonoBehaviour
    {
        private Outline[] _outlines;
        public float outlineThickness;
        public Color outlineColor;

        private void Awake()
        {
            _outlines = transform.GetComponentsInChildren<Outline>();
            SetOutlineActive(false);
        }

        private void Update()
        {
            UpdateOutlines();
        }


        public void SetOutlineActive(bool value)
        {
            _outlines.ApplyAction(o => o.enabled = value);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _outlines = _outlines ?? transform.GetComponentsInChildren<Outline>();
            UpdateOutlines();
        }

        private void UpdateOutlines()
        {
            _outlines.ApplyAction(o =>
            {
                o.OutlineWidth = outlineThickness;
                o.OutlineColor = outlineColor;
            });
        }
#endif
    }
}