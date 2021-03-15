using System;
using System.Diagnostics.Tracing;
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
        public static bool isGamePaused;


        private void Awake()
        {
            gameObject.SetActive(false);


            EventManager.AddListener<Action>("Game/Pause/TogglePause", () =>
            {
                if (isGamePaused)
                {
                    Resume();
                    return;
                }

                Pause();
            });
        }

        public static void TogglePause()
        {
            EventManager.TriggerEvent("Game/Pause/TogglePause");
        }


        public void Pause()
        {
            CameraController.SetCursorActive(owner.gameObject, true);
            _currentTimeScale = Time.timeScale;
            gameObject.SetActive(true);
            Time.timeScale = 0;
            isGamePaused = true;
        }

        public void Resume()
        {
            CameraController.SetCursorActive(owner.gameObject, false);
            Time.timeScale = _currentTimeScale;
            gameObject.SetActive(false);
            isGamePaused = false;
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