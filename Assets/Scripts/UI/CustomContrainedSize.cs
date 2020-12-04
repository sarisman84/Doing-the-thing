using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    [RequireComponent(typeof(ContentSizeFitter))]
    [ExecuteInEditMode]
    public class CustomContrainedSize : MonoBehaviour
    {
        private ContentSizeFitter _fitter;
        public TMP_Text textField;
        public int letterCount;


        private void Update()
        {
            _fitter.enabled = textField.text.Length < letterCount;
        }
    }
}
