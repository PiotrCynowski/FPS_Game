using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements {
    public class PlayerStateUI : MonoBehaviour
    {
        [SerializeField] private Text playerStateText;


        private void Start()
        {
            playerStateText.text = playerStateText.text = "PLAYER STATE: Idle";
        }


        #region enable/disable
        private void OnEnable()
        {
            PlayerState.OnPlayerStateChanged += UpdatePlayerState;
        }

        private void OnDisable()
        {
            PlayerState.OnPlayerStateChanged -= UpdatePlayerState;
        }
        #endregion


        public void UpdatePlayerState(PlayerState.PlayerDefinedStates _playerStateInfo)
        {
            playerStateText.text = "PLAYER STATE: " + _playerStateInfo.ToString();
        }
    }
}