using System.Collections;
using UnityEngine;

public class SideDoorPatrolObstacleEvent : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource doorAudioSource;

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
    public Transform entryPoint;

    [Header("Trigger Settings")]
    public float triggerDistanceZ = 12f;

    [Header("Door Settings")]
    public DoorOpenAxis doorOpenAxis = DoorOpenAxis.Y;
    public float openAngle = 90f;
    public float doorOpenSpeed = 180f;

    [Header("Obstacle Entry")]
    public float obstacleMoveToEntrySpeed = 8f;
    public float obstacleMoveDelay = 0.15f;

    [Header("Patrol Settings")]
    public float leftPatrolX = -2.5f;
    public float rightPatrolX = 2.5f;
    public float patrolSpeed = 5f;

    private bool hasTriggered = false;
    private bool isOpeningDoor = false;
    private bool isMovingToEntry = false;
    private bool isPatrolling = false;

    private Vector3 closedLocalEuler;
    private float closedAxisAngle;
    private float targetAxisAngle;

    private int patrolDirection = 1; // 1 = right, -1 = left

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
            UpdateMoveToEntry();
            UpdatePatrol();
            return;
        }

        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
        {
            return;
        }

        if (player == null || doorPivot == null || obstacle == null || entryPoint == null)
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

        if (doorAudioSource != null && !doorAudioSource.isPlaying)
        {
            doorAudioSource.Play();
        }

        StartCoroutine(BeginObstacleMoveAfterDelay());
    }

    private IEnumerator BeginObstacleMoveAfterDelay()
    {
        yield return new WaitForSeconds(obstacleMoveDelay);
        isMovingToEntry = true;
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

    private void UpdateMoveToEntry()
    {
        if (!isMovingToEntry || obstacle == null || entryPoint == null)
        {
            return;
        }

        obstacle.position = Vector3.MoveTowards(
            obstacle.position,
            entryPoint.position,
            obstacleMoveToEntrySpeed * Time.deltaTime
        );

        if (Vector3.Distance(obstacle.position, entryPoint.position) < 0.01f)
        {
            obstacle.position = entryPoint.position;
            isMovingToEntry = false;
            isPatrolling = true;

            float distanceToLeft = Mathf.Abs(obstacle.position.x - leftPatrolX);
            float distanceToRight = Mathf.Abs(obstacle.position.x - rightPatrolX);
            patrolDirection = distanceToRight < distanceToLeft ? -1 : 1;
        }
    }

    private void UpdatePatrol()
    {
        if (!isPatrolling || obstacle == null)
        {
            return;
        }

        Vector3 pos = obstacle.position;
        pos.x += patrolDirection * patrolSpeed * Time.deltaTime;

        if (pos.x >= rightPatrolX)
        {
            pos.x = rightPatrolX;
            patrolDirection = -1;
        }
        else if (pos.x <= leftPatrolX)
        {
            pos.x = leftPatrolX;
            patrolDirection = 1;
        }

        obstacle.position = pos;
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