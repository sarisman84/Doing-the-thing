using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity.Components.General
{
    public class PolymorphicEntity : MonoBehaviour
    {
        public UnityEvent onEarlyPolymorph;
        public UnityEvent onLatePolymorph;
        public bool IsAlreadyPolymorphed { get; private set; }
        public void PolymorphEntity(GameObject newModel)
        {
            if (IsAlreadyPolymorphed) return;
            onEarlyPolymorph?.Invoke();
            Transform currentModelHolder = transform.GetChild(0);
            if (!currentModelHolder) return;

           Transform currentModel = currentModelHolder.GetChild(0);
            
            currentModelHolder.gameObject.SetActive(false);
            GameObject model = Instantiate(newModel, currentModel);
            model.transform.localPosition = Vector3.zero;
            model.transform.rotation = Quaternion.identity;
            IsAlreadyPolymorphed = true;
            onLatePolymorph?.Invoke();
        }
    }
}