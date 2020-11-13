using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CustomUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<CustomUIElement> OnEnteringUIElement;
        public event Action<CustomUIElement> OnExitingUIElement;
        public event Action OnClickUIElement;
        
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            OnEnteringUIElement?.Invoke(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            OnExitingUIElement?.Invoke(this);
        }


        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnClickUIElement?.Invoke();
        }
    }
}