using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements {
    public class PlayerStateUI : MonoBehaviour
    {
        [SerializeField] private Text playerStateText;

        private void OnEnable()
        {
            PlayerState.OnPlayerStateChanged += UpdatePlayerState;
        }

        private void Start()
        {
            playerStateText.text = playerStateText.text = "PLAYER STATE: Idle";
        }

        private void OnDisable()
        {
            PlayerState.OnPlayerStateChanged -= UpdatePlayerState;
        }

        public void UpdatePlayerState(PlayerState.PlayerDefinedStates _playerStateInfo)
        {
            playerStateText.text = "PLAYER STATE: " + _playerStateInfo.ToString();
        }
    }
}