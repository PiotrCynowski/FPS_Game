using System.Collections;
using UnityEngine;
using System;
using UI.Elements;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        private float currentSpeed;
        [SerializeField] private float targetSpeed = 35f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float groundDrag = 5f;

        private Vector3 moveDirection;
        private Vector3 slopeMoveDirection;
        private Vector3 detectAbyssPos;

        private PlayerState stateOfPlayer;

        [Header("Settings")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float groundDistance = 0.2f;
        [SerializeField] private float playerHeight = 2f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Transform groundCheck;

        private bool isGrounded;
        private bool isSlope;
        private Rigidbody rb;
        private RaycastHit slopeHit;
        private float rayDistance;

        private float AbyssDistanceCheck = 1.5f;

        [Header("PowerUps")]
        private float movementBoostDuration;
        private Coroutine movementBoost;
        [Range(0, 100)]
        [SerializeField] private float powerupPercentageMultiply = 50f;
        private float powerupMultiply;
        private float powerupDivide;

        private void OnEnable()
        {
            PlayerInteractions.onMovementBoost += EnablePowerup;
            PanelPauseUI.OnPlayerReset += ResetPlayerPosition;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            rayDistance = playerHeight / 2 + 0.5f;

            CalculatePowerupValues();

            stateOfPlayer = new PlayerState();

            Vector3 playerPos = transform.position;
            PlayerPrefs.SetFloat("playerPosX", playerPos.x);
            PlayerPrefs.SetFloat("playerPosY", playerPos.y);
            PlayerPrefs.SetFloat("playerPosZ", playerPos.z);
        }

        private void Update()
        {
            if (isSlope)
                slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

            rb.drag = isGrounded ? groundDrag : 0;

            PlayerMovementState();
        }

        private void FixedUpdate()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            MovePlayer();
        }

        private void OnDisable()
        {
            PlayerInteractions.onMovementBoost -= EnablePowerup;
            PanelPauseUI.OnPlayerReset -= ResetPlayerPosition;
        }

        public void ReceiveInput(Vector2 _horizontalInput)
        {
            moveDirection = transform.right * _horizontalInput.x + transform.forward * _horizontalInput.y;
        }

        #region PowerUp
        private void CalculatePowerupValues()
        {
            powerupMultiply = 1 + (powerupPercentageMultiply / 100);
            powerupDivide = 1 / powerupMultiply;
        }

        private void EnablePowerup(int _addTimeToPuDuration)
        {
            movementBoostDuration += _addTimeToPuDuration;

            if (movementBoost == null)
            {
                movementBoost = StartCoroutine(MovementBoostRoutine());
            }
        }

        private IEnumerator MovementBoostRoutine()
        {
            targetSpeed *= powerupMultiply;
            jumpForce *= powerupMultiply;
            AbyssDistanceCheck *= 2;

            while (movementBoostDuration > 0)
            {
                movementBoostDuration -= Time.deltaTime;
                yield return null;
            }

            targetSpeed *= powerupDivide;
            jumpForce *= powerupDivide;
            AbyssDistanceCheck *= 0.5f;
            movementBoost = null;
        }
        #endregion

        public void OnJumpPressed()
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        #region Player Ground Move
        private void ResetPlayerPosition()
        {
            float x = PlayerPrefs.GetFloat("playerPosX", 0f);
            float y = PlayerPrefs.GetFloat("playerPosY", 0f);
            float z = PlayerPrefs.GetFloat("playerPosZ", 0f);
            transform.position = new Vector3(x, y, z);
        }

        private void MovePlayer()
        {
            isSlope = DetectSlope();

            if (isGrounded && !isSlope)
            {          
                if (IsNotAbyss())
                {
                    rb.AddForce(moveDirection.normalized * currentSpeed, ForceMode.Acceleration);
                }
            }
            else if (isGrounded && isSlope)
            {            
                rb.AddForce(slopeMoveDirection.normalized * currentSpeed, ForceMode.Acceleration);
            }
        }

        private void PlayerMovementState()
        {
            if (isGrounded)
            {
                if (moveDirection.magnitude > 0)
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                }
                else
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime); // Smooth deceleration
                }
            }

            stateOfPlayer.CurrentState = isGrounded ? (moveDirection.magnitude > 0 ? PlayerState.PlayerDefinedStates.Walking : PlayerState.PlayerDefinedStates.Idle) : PlayerState.PlayerDefinedStates.Jumping;
        }

        private bool IsNotAbyss()
        {
            detectAbyssPos = groundCheck.position + moveDirection.normalized * AbyssDistanceCheck;
            return Physics.Raycast(detectAbyssPos, Vector3.down, rayDistance);
        }

        private bool DetectSlope()
        {
           return Physics.Raycast(transform.position, Vector3.down, out slopeHit, rayDistance, groundMask) && slopeHit.normal != Vector3.up;
        }
        #endregion
    }
}