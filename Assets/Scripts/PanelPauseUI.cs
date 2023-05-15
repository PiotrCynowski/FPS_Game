using UnityEngine;
using UnityEngine.UI;
using GameInput;
using System;

namespace UI.Elements
{
    public class PanelPauseUI : MonoBehaviour
    {
        [SerializeField] private Button buttonBackFromPause;
        [SerializeField] private Button buttonExitApp;

        [SerializeField] private GameObject panelPause;
        public static Action OnPlayerPauseMenuOff;

        private void Start()
        {
            buttonBackFromPause.onClick.AddListener(() => BackFromPause());
            buttonExitApp.onClick.AddListener(() => ExitGame());
        }

        #region enable/disable
        private void OnEnable()
        {
            InputManager.onPlayerEscButton += PauseGame;
        }

        private void OnDisable()
        {
            InputManager.onPlayerEscButton -= PauseGame;
        }
        #endregion


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

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}
