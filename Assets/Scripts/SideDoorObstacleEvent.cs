using System.Collections;
using UnityEngine;

public class SideDoorObstacleEvent : MonoBehaviour
{
    public enum DoorOpenAxis
    {
        X,
        Y,
        Z
    }

    [Header("References")]
    public Transform player;
    public Transform doorPivot;
    public Transform obstacle;
    public Transform targetPoint;

    [Header("Trigger Settings")]
    public float triggerDistanceZ = 12f;

    [Header("Door Settings")]
    public DoorOpenAxis doorOpenAxis = DoorOpenAxis.Y;
    public float openAngle = 90f;
    public float doorOpenSpeed = 360f;

    [Header("Obstacle Movement")]
    public float obstacleMoveSpeed = 8f;
    public float obstacleMoveDelay = 0.15f;

    private bool hasTriggered = false;
    private bool isOpeningDoor = false;
    private bool isMovingObstacle = false;

    private Vector3 closedLocalEuler;
    private float closedAxisAngle;
    private float targetAxisAngle;

    private void Start()
    {
        if (doorPivot != null)
        {
            closedLocalEuler = doorPivot.localEulerAngles;
            closedAxisAngle = GetAxisValue(closedLocalEuler);
            targetAxisAngle = closedAxisAngle + openAngle;
        }
    }

    private void Update()
    {
        if (hasTriggered)
        {
            UpdateDoorOpening();
            UpdateObstacleMovement();
            return;
        }

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            return;
        }

        if (player == null || doorPivot == null || obstacle == null || targetPoint == null)
        {
            return;
        }

        float zDifference = obstacle.position.z - player.position.z;

        if (zDifference <= triggerDistanceZ)
        {
            TriggerEvent();
        }
    }

    private void TriggerEvent()
    {
        hasTriggered = true;
        isOpeningDoor = true;
        StartCoroutine(BeginObstacleMoveAfterDelay());
    }

    private IEnumerator BeginObstacleMoveAfterDelay()
    {
        yield return new WaitForSeconds(obstacleMoveDelay);
        isMovingObstacle = true;
    }

    private void UpdateDoorOpening()
    {
        if (!isOpeningDoor || doorPivot == null)
        {
            return;
        }

        Vector3 euler = doorPivot.localEulerAngles;
        float currentAxisAngle = GetAxisValue(euler);

        float newAxisAngle = Mathf.MoveTowardsAngle(
            currentAxisAngle,
            targetAxisAngle,
            doorOpenSpeed * Time.deltaTime
        );

        euler = SetAxisValue(euler, newAxisAngle);
        doorPivot.localEulerAngles = euler;

        if (Mathf.Abs(Mathf.DeltaAngle(newAxisAngle, targetAxisAngle)) < 0.1f)
        {
            doorPivot.localEulerAngles = SetAxisValue(doorPivot.localEulerAngles, targetAxisAngle);
            isOpeningDoor = false;
        }
    }

    private void UpdateObstacleMovement()
    {
        if (!isMovingObstacle || obstacle == null || targetPoint == null)
        {
            return;
        }

        obstacle.position = Vector3.MoveTowards(
            obstacle.position,
            targetPoint.position,
            obstacleMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(obstacle.position, targetPoint.position) < 0.01f)
        {
            obstacle.position = targetPoint.position;
            isMovingObstacle = false;
        }
    }

    private float GetAxisValue(Vector3 euler)
    {
        switch (doorOpenAxis)
        {
            case DoorOpenAxis.X:
                return euler.x;
            case DoorOpenAxis.Y:
                return euler.y;
            case DoorOpenAxis.Z:
                return euler.z;
            default:
                return euler.y;
        }
    }

    private Vector3 SetAxisValue(Vector3 euler, float value)
    {
        switch (doorOpenAxis)
        {
            case DoorOpenAxis.X:
                euler.x = value;
                break;
            case DoorOpenAxis.Y:
                euler.y = value;
                break;
            case DoorOpenAxis.Z:
                euler.z = value;
                break;
        }

        return euler;
    }
}