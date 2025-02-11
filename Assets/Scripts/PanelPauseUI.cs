using UnityEngine;
using UnityEngine.UI;
using GameInput;
using System;

namespace UI.Elements
{
    public class PanelPauseUI : MonoBehaviour
    {
        [SerializeField] private Button buttonBackFromPause;
        [SerializeField] private Button buttonRestart;
        [SerializeField] private Button buttonExitApp;

        [SerializeField] private GameObject panelPause;
        public static Action OnPlayerPauseMenuOff;
        public static Action OnPlayerRestart;

        private void OnEnable()
        {
            InputManager.onPlayerEscButton += PauseGame;
        }

        private void Start()
        {
            buttonBackFromPause.onClick.AddListener(() => BackFromPause());
            buttonRestart.onClick.AddListener(() => RestartGame());
            buttonExitApp.onClick.AddListener(() => ExitGame());
        }

        private void OnDisable()
        {
            InputManager.onPlayerEscButton -= PauseGame;
        }

        private void PauseGame()
        {
            panelPause.SetActive(true);
            Time.timeScale = 0;
        }

        private void BackFromPause()
        {
            panelPause.SetActive(false);
            Time.timeScale = 1;

            OnPlayerPauseMenuOff?.Invoke();
        }

        private void RestartGame()
        {
            BackFromPause();
            OnPlayerRestart?.Invoke();
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}