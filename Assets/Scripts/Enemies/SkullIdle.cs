using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullIdle : SkullState
{
    readonly SkullMovement skull;
    private GameObject player;
    private Vector3 direction;
    private Vector3 sideDirection;
    private float idleTime;
    public SkullIdle(SkullMovement skull, GameObject player)
    {
        this.skull = skull;
        this.player = player;
        skull.direction = Direction.SIDE;
        direction = (player.transform.position - skull.transform.position).normalized;
        Utils.TravelDestinationCorrection(direction, player.transform.position, skull.minDistanceToGround, skull.maxDistanceToGround);
        sideDirection = Vector3.Cross(direction, Vector3.up);
        idleTime = skull.idleTime;
    }

    public void Action()
    {
        idleTime -= Time.deltaTime;

        skull.Move(direction * skull.idleSpeed * Time.deltaTime);
        skull.SmoothLook(sideDirection, skull.idleRotationSpeed);

        if (idleTime <= 0)
        {
            skull.state = new DirectionPickerState(skull, player);
        }
    }
}
