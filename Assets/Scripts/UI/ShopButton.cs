using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopButton : CustomUIElement
    {
        public Guid ID { get; set; }

        public Sprite ShopIcon
        {
            set
            {
                _image = _image != null ? _image : transform.GetChild(0).GetComponent<Image>();
                _image.sprite = value;
            }
        }

        private Image _image;
    }
}