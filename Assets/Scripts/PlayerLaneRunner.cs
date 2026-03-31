using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerLaneRunner : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 8f;

    [Header("Lane Movement")]
    public float laneOffset = 3.5f;
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

    private CharacterController controller;
    private float verticalVelocity;

    private Camera playerCamera;
    private bool isCrouching = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingCenterY, 0f);

        if (playerCamera != null)
        {
            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.y = standingCameraY;
            playerCamera.transform.localPosition = camPos;
        }
    }

    private void Update()
    {
        HandleCrouchState();

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            ApplyGroundedGravityOnly();
            UpdateCrouchVisuals();
            return;
        }

        HandleLaneInput();
        HandleJumpInput();
        HandleMovement();
        UpdateCrouchVisuals();
    }

    private void HandleLaneInput()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                currentLane--;
                currentLane = Mathf.Clamp(currentLane, 0, 2);
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                currentLane++;
                currentLane = Mathf.Clamp(currentLane, 0, 2);
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
        }
    }

    private void HandleCrouchState()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        isCrouching = Keyboard.current.sKey.isPressed;
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

        Vector3 move = new Vector3(moveX, verticalVelocity, forwardSpeed);
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

    private void UpdateCrouchVisuals()
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
            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, crouchTransitionSpeed * Time.deltaTime);
            playerCamera.transform.localPosition = camPos;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Obstacle obstacle = hit.gameObject.GetComponent<Obstacle>();

        if (obstacle != null)
        {
            Debug.Log("Hit obstacle type: " + obstacle.obstacleType);
        }
    }
}