using System;
using System.Collections;
using System.Collections.Generic;
using Interactivity.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility.Attributes;

namespace Utility.TestScripts
{
    public class TestComponent : MonoBehaviour
    {
        [Expose] public ScriptableObject customEvent;
        [Expose] public ScriptableObject someOtherObject;

        public GameObject shopPanel;
        private Color originalColor;


        public void PublicOpenShop()
        {
            originalColor = shopPanel.GetComponent<Image>().color;
            StartCoroutine(DelayOpenShop());
        }

        public void PublicCloseShop()
        {
            StartCoroutine(DelayCloseShop());
        }

        IEnumerator DelayOpenShop()
        {
            shopPanel.SetActive(true);
            shopPanel.GetComponent<Image>().color = new Color();


            while (shopPanel.GetComponent<Image>().color != originalColor)
            {
                yield return new WaitForEndOfFrame();
                shopPanel.GetComponent<Image>().color =
                    Color.Lerp(shopPanel.GetComponent<Image>().color, originalColor, 0.5f);
            }
        }

        IEnumerator DelayCloseShop()
        {
            while (shopPanel.GetComponent<Image>().color != new Color())
            {
                yield return new WaitForEndOfFrame();
                shopPanel.GetComponent<Image>().color =
                    Color.Lerp(originalColor, new Color(), 0.5f);
            }

            shopPanel.SetActive(false);
            shopPanel.GetComponent<Image>().color = new Color();
        }
    }
}