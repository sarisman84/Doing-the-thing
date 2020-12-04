using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuestUIElement : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text description;
        public Image image;
        public bool IsBeingUsed => QuestID != default;
        public Guid QuestID {  get; set; }

        public void ResetText()
        {
            title.text = string.Empty;
            description.text = string.Empty;
            image.enabled = false;
        }
    }
}