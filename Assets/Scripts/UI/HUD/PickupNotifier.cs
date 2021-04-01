using System.Collections;
using System.Collections.Generic;
using Extensions;
using Spyro.Optimisation.ObjectManagement;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class PickupNotifier
    {
        private TMP_Text _uiPrefab;
        private CanvasGroup _uiParent;
        private List<TMP_Text> _flaggedForResetUIElemenets = new List<TMP_Text>();
        private float _resetTimer;
        private int _activePickupMessageAmm;

        public PickupNotifier(TMP_Text uiPrefab, CanvasGroup uiParent, int activePickupMessageAmm)
        {
            _uiPrefab = uiPrefab;
            _uiParent = uiParent;
            _activePickupMessageAmm = activePickupMessageAmm;
            CoroutineManager.Instance.DynamicStartCoroutine(OnUpdate());
            ObjectManager.PoolGameObject(_uiPrefab, 100, _uiParent.transform);
            Debug.Log("Initialized PickupNotifier");
        }

        private IEnumerator OnUpdate()
        {
            Start:
            while (Application.isPlaying)
            {
                _resetTimer += Time.deltaTime;
                if (_resetTimer >= 1)
                {
                    ResetPickupMessage();
                    _resetTimer = 0;
                    goto Start;
                }

                //If the current message elements exceed the assigned limit, remove the older elements.
                while (_flaggedForResetUIElemenets.Count > _activePickupMessageAmm)
                {
                    TMP_Text messageElement = _flaggedForResetUIElemenets[0];
                    messageElement.text = "";
                    messageElement.gameObject.SetActive(false);
                    _flaggedForResetUIElemenets.Remove(messageElement);
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        private void ResetPickupMessage()
        {
            foreach (var messageElement in _flaggedForResetUIElemenets)
            {
                messageElement.text = "";
                messageElement.gameObject.SetActive(false);
            }

            _flaggedForResetUIElemenets.Clear();
        }


        public void PickupMessageEvent(string message)
        {
            TMP_Text messageElement = ObjectManager.DynamicComponentInstantiate(_uiPrefab);
            messageElement.gameObject.SetActive(true);
            messageElement.text = $"Picked up {message}";
            _flaggedForResetUIElemenets.Add(messageElement);
            _resetTimer = 0;
        }
    }
}