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

    [Header("Gravity")]
    public float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleLaneInput();
        HandleMovement();
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.GetComponent<Obstacle>() != null)
        {
            Debug.Log("Hit obstacle!");
        }
    }
}