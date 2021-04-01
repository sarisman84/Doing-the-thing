using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public class CoroutineManager : MonoBehaviour
    {
        public static CoroutineManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameStart()
        {
            Instance = new GameObject("Coroutine Manager").AddComponent<CoroutineManager>();
        }

        private Dictionary<IEnumerator, Coroutine> currentCoroutines = new Dictionary<IEnumerator, Coroutine>();


        public void DynamicStartCoroutine(IEnumerator coroutine)
        {
            KeyValuePair<IEnumerator, Coroutine> foundCoroutine =
                currentCoroutines.FirstOrDefault(c => c.Key == coroutine);
            if (foundCoroutine.Key == null && foundCoroutine.Value == null)
            {
                currentCoroutines.Add(coroutine, StartCoroutine(coroutine));
            }
            else
            {
                if (foundCoroutine.Value != null)
                    StopCoroutine(currentCoroutines[coroutine]);
                currentCoroutines[coroutine] = StartCoroutine(coroutine);
            }
        }
    }
}