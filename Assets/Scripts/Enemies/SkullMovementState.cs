using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullMovementState : SkullState
{
    readonly SkullMovement skull;
    private TravelBox travelBox;
    private Vector3 travelDestination;
    private GameObject player;

    public SkullMovementState(SkullMovement skull, GameObject player)
    {
        this.skull = skull;
        this.player = player;
        TravelBox[] travelBoxes = Utils.GenerateTravelBoxes(skull.direction, skull.transform, skull.movementLength, skull.movementWidth, skull.movementHeight,
            skull.minMovementLength, skull.minDistanceToGround, skull.maxDistanceToGround);
        travelBox = skull.direction == Direction.LEFT ? travelBoxes[1] : travelBoxes[0];
        travelDestination = Utils.GetRandomPointInBox(new Vector3(skull.movementWidth, skull.movementHeight, skull.movementLength), travelBox.center, travelBox.rotation);

        travelDestination = Utils.TravelDestinationCorrection(travelDestination, player.transform.position, skull.minDistanceToGround, skull.maxDistanceToGround);
        if (SphereBoundaries.IsPointWithinSphere(SphereBoundaries.minDistanceBound, travelDestination))
        {
            travelDestination = SphereBoundaries.MovePointOutsideMinDistanceZone(travelDestination);
            travelDestination = Utils.TravelDestinationCorrection(travelDestination, player.transform.position, skull.minDistanceToGround, skull.maxDistanceToGround);
        }
    }

    public void Action()
    {
        Vector3 orientation = (player.transform.position - skull.transform.position).normalized;
        orientation = skull.direction == Direction.LEFT ? -orientation : orientation;
        skull.SmoothLook(orientation);

        //For now Ill do a simple movement here
        skull.transform.position = Vector3.MoveTowards(skull.transform.position, travelDestination, skull.movementSpeed * Time.deltaTime);

        if (Vector3.Distance(skull.transform.position, travelDestination) < 0.1f)
        {
            if (SphereBoundaries.IsPointWithinSphere(SphereBoundaries.attackBound, skull.transform.position))
            {
                skull.state = new SkullAttackState(skull, player);
            }
            else
            {
                skull.state = new SkullIdle(skull, player);
            }
        }
    }
}

