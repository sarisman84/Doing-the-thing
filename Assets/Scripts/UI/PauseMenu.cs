using System;
using System.Diagnostics.Tracing;
using Interactivity.Events;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public PlayerController owner;
        private float _currentTimeScale;
        private bool _isCurrentlyPaused;

        private static CustomEvent onTogglePause;

        private void OnEnable()
        {
            onTogglePause = CustomEvent.CreateEvent<Func<bool>>(Pause, owner.gameObject);
        }

        private void OnDisable()
        {
            onTogglePause.RemoveEvent<Func<bool>>(Pause);
        }

        private void Awake()
        {
            gameObject.SetActive(false);


            // EventManager.AddListener<Action>("Game/Pause/TogglePause", () =>
            // {
            //     if (isGamePaused)
            //     {
            //         Resume();
            //         return;
            //     }
            //
            //     Pause();
            // });
        }

        public static bool TogglePause(GameObject owner)
        {
            if (onTogglePause)
                return (bool) onTogglePause.OnInvokeEvent(owner, null);
            return false;
        }


        public bool Pause()
        {
            _isCurrentlyPaused = !_isCurrentlyPaused;

            if (_isCurrentlyPaused)
            {
                CameraController.SetCursorActive(owner.gameObject, true);
                _currentTimeScale = Time.timeScale;
                gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
                Resume();


            return _isCurrentlyPaused;
        }

        public void Resume()
        {
            CameraController.SetCursorActive(owner.gameObject, false);
            Time.timeScale = _currentTimeScale;
            gameObject.SetActive(false);
        }

        public void ToMainMenu()
        {
            Resume();
            CameraController.SetCursorActive(owner.gameObject, true);
            Time.timeScale = 1f;
            //SceneManager.LoadScene("Game_MainMenu", LoadSceneMode.Single);
            Debug.Log("Loading Menu...");
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}