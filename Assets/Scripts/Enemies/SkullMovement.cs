using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum Direction
{
    LEFT,
    RIGHT,
    SIDE
}
public class SkullMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] public int directionChangeChance = 70;
    [SerializeField] public float movementWidth = 5;
    [SerializeField] public float movementLength = 5;
    [SerializeField] public float movementHeight = 5;
    [SerializeField] public float minMovementLength = 1;
    [SerializeField] public float minDistanceToGround = 1.5f;
    [SerializeField] public float maxDistanceToGround = 10;
    [SerializeField] public float idleSpeed = 1;
    [SerializeField] public float idleTime = 1;
    [SerializeField] public float idleRotationSpeed = 5;
    [SerializeField] public float movementSpeed = 4;

    public Direction direction;
    public SkullState state;

    void Start()
    {
        state = new DirectionPickerState(this, player);
    }

    void Update()
    {
        state.Action();
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction;
    }

    public void SmoothLook(Vector3 direction, float speed = 1)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OnDrawGizmos()
    {
        Vector3 lookDirection = transform.forward;
        TravelBox[] travelBoxes = Utils.GenerateTravelBoxes(direction, transform, movementLength, movementWidth, movementHeight, minMovementLength, minDistanceToGround, maxDistanceToGround);
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