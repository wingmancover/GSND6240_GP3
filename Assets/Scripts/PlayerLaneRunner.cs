using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerLaneRunner : MonoBehaviour
{
    [Header("Forward Movement")]
    public float baseForwardSpeed = 7f;
    public float maxForwardSpeed = 11f;

    [Header("Speed Ramp")]
    public float speedIncreaseStartDelay = 10f;
    public float speedIncreaseAmount = 0.2f;
    public float speedIncreaseInterval = 0.7f;

    [Header("Lane Movement")]
    public float laneOffset = 2.5f;
    public float laneChangeSpeed = 12f;
    public int currentLane = 1; // 0 = left, 1 = middle, 2 = right

    [Header("Jump Settings")]
    public float jumpForce = 8f;

    [Header("Gravity")]
    public float gravity = -24f;

    [Header("Crouch Settings")]
    public float standingHeight = 1.8f;
    public float crouchingHeight = 1.0f;
    public float standingCenterY = 0.9f;
    public float crouchingCenterY = 0.5f;
    public float standingCameraY = 1.27f;
    public float crouchingCameraY = 0.57f;
    public float crouchTransitionSpeed = 12f;

    [Header("Hit / Game Over Settings")]
    public int maxHits = 4;
    public float invincibilityDuration = 3f;
    public float hitShakeDuration = 0.6f;
    public float hitShakeMagnitude = 0.08f;


    private CharacterController controller;
    private float verticalVelocity;

    private Camera playerCamera;
    private bool isCrouching = false;

    private float currentForwardSpeed;
    private float gameplayTimer;
    private float speedIncreaseTimer;

    private int currentHitCount = 0;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private float shakeTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingCenterY, 0f);

        if (playerCamera != null)
        {
            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.x = 0f;
            camPos.y = standingCameraY;
            camPos.z = 0f;
            playerCamera.transform.localPosition = camPos;
        }

        currentForwardSpeed = baseForwardSpeed;
        gameplayTimer = 0f;
        speedIncreaseTimer = 0f;
    }

    private void Update()
    {
        UpdateInvincibilityTimer();

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            ApplyGroundedGravityOnly();
            UpdateCrouchAndCamera();
            return;
        }

        HandleCrouchState();
        UpdateSpeedRamp();

        HandleLaneInput();
        HandleJumpInput();
        HandleMovement();
        UpdateCrouchAndCamera();
    }

    private void HandleLaneInput()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                currentLane--;
                currentLane = Mathf.Clamp(currentLane, 0, 2);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayWhoosh();
                }
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                currentLane++;
                currentLane = Mathf.Clamp(currentLane, 0, 2);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayWhoosh();
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded && !isCrouching)
        {
            verticalVelocity = jumpForce;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
        }
    }

    private void HandleCrouchState()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
        }

        isCrouching = Keyboard.current.sKey.isPressed;
    }

    private void UpdateSpeedRamp()
    {
        gameplayTimer += Time.deltaTime;

        if (gameplayTimer < speedIncreaseStartDelay)
        {
            return;
        }

        if (currentForwardSpeed >= maxForwardSpeed)
        {
            currentForwardSpeed = maxForwardSpeed;
            return;
        }

        speedIncreaseTimer += Time.deltaTime;

        if (speedIncreaseTimer >= speedIncreaseInterval)
        {
            currentForwardSpeed += speedIncreaseAmount;
            currentForwardSpeed = Mathf.Min(currentForwardSpeed, maxForwardSpeed);
            speedIncreaseTimer = 0f;
        }
    }

    private void HandleMovement()
    {
        float targetX = (currentLane - 1) * laneOffset;
        float deltaX = targetX - transform.position.x;
        float moveX = deltaX * laneChangeSpeed;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = new Vector3(moveX, verticalVelocity, currentForwardSpeed);
        controller.Move(move * Time.deltaTime);
    }

    private void ApplyGroundedGravityOnly()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    private void UpdateInvincibilityTimer()
    {
        if (!isInvincible)
        {
            return;
        }

        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            invincibilityTimer = 0f;
        }
    }

    private void UpdateCrouchAndCamera()
    {
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        float targetCenterY = isCrouching ? crouchingCenterY : standingCenterY;
        float targetCameraY = isCrouching ? crouchingCameraY : standingCameraY;

        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);

        Vector3 center = controller.center;
        center.y = Mathf.Lerp(center.y, targetCenterY, crouchTransitionSpeed * Time.deltaTime);
        controller.center = center;

        if (playerCamera != null)
        {
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
            }

            float shakeX = 0f;
            float shakeY = 0f;

            if (shakeTimer > 0f)
            {
                shakeX = Random.Range(-hitShakeMagnitude, hitShakeMagnitude);
                shakeY = Random.Range(-hitShakeMagnitude, hitShakeMagnitude);
            }

            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.x = Mathf.Lerp(camPos.x, shakeX, crouchTransitionSpeed * Time.deltaTime);
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY + shakeY, crouchTransitionSpeed * Time.deltaTime);
            camPos.z = 0f;
            playerCamera.transform.localPosition = camPos;
        }
    }

    private void RegisterHit(Obstacle obstacle)
    {
        if (isInvincible)
        {
            return;
        }

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        currentHitCount++;
        shakeTimer = hitShakeDuration;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHitUI(currentHitCount);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHitObstacle();
        }

        Debug.Log("Hit obstacle type: " + obstacle.obstacleType);
        Debug.Log("Current hit count: " + currentHitCount + " / " + maxHits);

        if (currentHitCount >= maxHits)
        {
            isInvincible = false;
            invincibilityTimer = 0f;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            return;
        }

        Obstacle obstacle = hit.gameObject.GetComponent<Obstacle>();

        if (obstacle != null)
        {
            RegisterHit(obstacle);
        }
    }
}