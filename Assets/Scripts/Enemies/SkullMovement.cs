using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
    LEFT,
    RIGHT
}

public class SkullMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int directionChangeChance = 70;
    [SerializeField] private float movementWidth = 5;
    [SerializeField] private float movementLength = 5;
    [SerializeField] private float movementHeight = 5;
    [SerializeField] private float minMovementLength = 1;
    [SerializeField] private float minDistanceToGround = 1.5f;
    [SerializeField] private float maxDistanceToGround = 10;

    private Direction direction = Direction.RIGHT; // Default direction

    void Start()
    {
        direction = Math.Round(UnityEngine.Random.Range(0f, 1f)) == 0 ? Direction.LEFT : Direction.RIGHT;
        Debug.Log(direction);
        LookAtPlayer();
    }
    void Update()
    {

        LookAtPlayer();
    }

    private void ChangeDirection()
    {
        if (UnityEngine.Random.Range(0, 100) < directionChangeChance)
        {
            direction = direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
        }
    }

    private void LookAtPlayer()
    {
        switch (direction)
        {
            case Direction.RIGHT:
                transform.LookAt(player.transform);
                break;
            case Direction.LEFT:
                transform.rotation = Quaternion.LookRotation(-(player.transform.position - transform.position));
                break;
        }
    }

    private TravelBox[] GetTravelBoxes()
    {
        Vector3 lookDirection = transform.forward;
        if (direction == Direction.LEFT)
        {
            lookDirection = -transform.forward;
        }
        float lengthAway = minMovementLength + (movementLength / 2);
        Vector3 right = transform.right * movementWidth / 2;
        Vector3 left = -right;

        if (direction == Direction.LEFT)
        {
            right = -right;
            left = -left;
        }

        Vector3 rightCenterRegion = lookDirection * lengthAway + transform.position + right;
        Vector3 leftCenterRegion = lookDirection * lengthAway + transform.position + left;

        TravelBox[] travelBoxes = new TravelBox[2];
        travelBoxes[0] = new TravelBox
        {
            center = rightCenterRegion,
            rotation = Quaternion.LookRotation(lookDirection)
        };

        travelBoxes[1] = new TravelBox
        {
            center = leftCenterRegion,
            rotation = Quaternion.LookRotation(lookDirection)
        };

        Vector3 rightLowestVertice = GetLowestVertice(travelBoxes[0]);

        if (rightLowestVertice.y < minDistanceToGround)
        {
            Vector3 upDirection = transform.up;
            float yDif = minDistanceToGround - rightLowestVertice.y;
            Vector3 toAdd = upDirection * (yDif / upDirection.y);
            travelBoxes[0].center += toAdd;
            travelBoxes[1].center += toAdd;

            return travelBoxes;
        }

        Vector3 rightHighestVertice = GetLowestVertice(travelBoxes[0], false);

        if (rightHighestVertice.y > maxDistanceToGround)
        {
            Vector3 upDirection = -transform.up;
            float yDif = rightHighestVertice.y - maxDistanceToGround;
            Vector3 toAdd = upDirection * (yDif / upDirection.y);
            travelBoxes[0].center -= toAdd;
            travelBoxes[1].center -= toAdd;
        }

        return travelBoxes;
    }

    private Vector3 GetLowestVertice(TravelBox travelBox, bool checkLowest = true)
    {
        Vector3 center = travelBox.center;
        Quaternion rotation = travelBox.rotation;

        float halfWidth = movementWidth / 2;
        float halfLength = movementLength / 2;
        float halfHeight = movementHeight / 2;

        halfHeight = checkLowest ? -halfHeight : halfHeight;

        Vector3[] localVertices = new Vector3[]
        {
            new Vector3(halfWidth, halfHeight, -halfLength),
            new Vector3(halfWidth, halfHeight, halfLength)
        };

        Vector3 lowestVertice = Vector3.positiveInfinity;

        for (int i = 0; i < localVertices.Length; i++)
        {
            Vector3 worldVertice = center + rotation * localVertices[i];
            if (worldVertice.y < lowestVertice.y)
            {
                lowestVertice = worldVertice;
            }
        }
        return lowestVertice;
    }

    private void OnDrawGizmos()
    {
        Vector3 lookDirection = transform.forward;
        TravelBox[] travelBoxes = GetTravelBoxes();
        TravelBox rightTravelBox = travelBoxes[0];
        TravelBox leftTravelBox = travelBoxes[1];

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(rightTravelBox.center, Quaternion.LookRotation(lookDirection), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(movementWidth, movementHeight, movementLength));

        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(leftTravelBox.center, Quaternion.LookRotation(lookDirection), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(movementWidth, movementHeight, movementLength));

        Gizmos.matrix = Matrix4x4.identity;
    }
}

class TravelBox
{
    public Vector3 center;
    public Quaternion rotation;
}
