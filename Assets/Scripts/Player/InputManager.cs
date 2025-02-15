using UnityEngine;
using Player;
using System;
using UI.Elements;

namespace GameInput
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerMouseLook mouseLook;
        [SerializeField] private PlayerWeapon weapon;

        private PlayerInputActions controls;
        private PlayerInputActions.PlayerActions playerActions;

        private Vector2 horizontalInput;
        private Vector2 mouseInput;

        public static Action onPlayerEscButton;

        private void Awake()
        {
            controls = new PlayerInputActions();
            playerActions = controls.Player;

            playerActions.Movement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

            playerActions.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            playerActions.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();

            playerActions.Shot1.performed += _ => weapon.ShotLeftMouseButton();
            playerActions.Shot2.performed += _ => weapon.ShotRightMouseButton();

            playerActions.PauseMenu.performed += _ => EscapeButtonPerformed();

            PanelPauseUI.OnPlayerPauseMenuOff += EnableControlls;
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void Update()
        {
            movement.ReceiveInput(horizontalInput);
            mouseLook.ReceiveInput(mouseInput);
        }

        private void OnDisable()
        {
            controls.Disable();
        }

        private void EscapeButtonPerformed()
        {
            onPlayerEscButton.Invoke();
            controls.Disable();
        }

        private void EnableControlls()
        {
            controls.Enable();
        }
    }
}