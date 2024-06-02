using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionPickerState : SkullState
{
    readonly SkullMovement skull;
    private GameObject player;
    private Vector3 direction;
    public DirectionPickerState(SkullMovement skull, GameObject player)
    {
        this.skull = skull;
        this.player = player;
        direction = (player.transform.position - skull.transform.position).normalized;
        ChangeDirection();
        direction = skull.direction == Direction.LEFT ? -direction : direction;
    }

    public void Action()
    {
        skull.SmoothLook(direction, skull.idleRotationSpeed);
        Vector3 forward = skull.transform.forward;
        forward = skull.direction == Direction.LEFT ? -forward : forward;
        if (Utils.IsLookingAt(skull.transform.position, player.transform.position, forward, 0.05f))
        {
            skull.state = new SkullMovementState(skull, player);
        }

    }

    private void ChangeDirection()
    {
        int random = UnityEngine.Random.Range(0, 100);
        skull.direction = random < 50 ? Direction.LEFT : Direction.RIGHT;
    }
}
