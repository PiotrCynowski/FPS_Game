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
        private readonly string savedPlayerPositionKey = "playerPosition";

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

            powerupMultiply = 1 + (powerupPercentageMultiply / 100);
            powerupDivide = 1 / powerupMultiply;

            stateOfPlayer = new PlayerState();

            Vector3 playerPos = transform.position;
            PlayerPrefs.SetString(savedPlayerPositionKey, $"{playerPos.x},{playerPos.y},{playerPos.z}");
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isSlope)
                slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

            rb.drag = isGrounded ? groundDrag : 0;

            PlayerMovementState();
        }

        private void FixedUpdate()
        {
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
        }
        #endregion

        public void OnJumpFlyPressed()
        {
            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Fly");
            }
        }

        public void OnCrouchPressed()
        {
            Debug.Log("Crouch");
        }

        #region Player Ground Move
        private void ResetPlayerPosition()
        {
            if (!PlayerPrefs.HasKey(savedPlayerPositionKey)) return;

            string[] values = PlayerPrefs.GetString(savedPlayerPositionKey).Split(',');
            if (values.Length == 3 &&
                float.TryParse(values[0], out float x) &&
                float.TryParse(values[1], out float y) &&
                float.TryParse(values[2], out float z))
            {
                transform.position = new Vector3(x, y, z);
            }
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
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            }

            if(moveDirection.magnitude == 0)
            {
                currentSpeed = 0;

                if (isGrounded)
                {
                    stateOfPlayer.CurrentState = PlayerState.PlayerDefinedStates.Idle;
                    return;
                }
            }

            stateOfPlayer.CurrentState = isGrounded ? PlayerState.PlayerDefinedStates.Walking : PlayerState.PlayerDefinedStates.Jumping;
        }

        private bool IsNotAbyss()
        {
            detectAbyssPos = groundCheck.position + moveDirection.normalized * AbyssDistanceCheck;
            return Physics.Raycast(detectAbyssPos, Vector3.down, rayDistance);
        }

        private bool DetectSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, rayDistance, groundMask))
            {
                if (slopeHit.normal != Vector3.up)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        #endregion
    }
}