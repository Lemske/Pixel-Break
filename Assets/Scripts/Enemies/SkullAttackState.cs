using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class SkullAttackState : SkullState
{
    readonly SkullMovement skull;
    protected GameObject player;
    protected Vector3 startingPosition;
    protected Vector3 attackPosition;
    protected Vector3 fallBackPosition;
    protected readonly List<Vector3> boundingVectors;
    Vector3 horizontalVector;
    Vector3 verticalVector;
    protected SkullState move;
    public SkullAttackState(SkullMovement skull, GameObject player)
    {
        this.skull = skull;
        this.player = player;
        boundingVectors = PlayerView.InBoundsVectors[0];
        horizontalVector = boundingVectors[0] - boundingVectors[1];
        verticalVector = boundingVectors[2] - boundingVectors[3];
        CalculatePositions();

        move = new Move1(this);
    }

    public void Action()
    {
        move.Action();
    }

    private void CalculatePositions() //Todo: Dont really like it but random enough
    {
        startingPosition = skull.transform.position;

        float horizontalPercentage = Random.Range(0f, 1f);
        float verticalPercentage = Random.Range(0.4f, 1f);

        Vector3 horizontalPosition = boundingVectors[1] + horizontalVector * horizontalPercentage;
        Vector3 verticalPosition = boundingVectors[3] + verticalVector * verticalPercentage;

        Vector3 combined = horizontalPosition + verticalPosition;

        Debug.DrawLine(SphereBoundaries.attackBound.center, combined * 10, Color.magenta, 10);
        Debug.DrawLine(SphereBoundaries.attackBound.center, verticalPosition * 10, Color.magenta, 10);

        Vector3 midPoint = ((startingPosition - SphereBoundaries.attackBound.center + combined) / 2).normalized;

        float rangeBetweenRadiuses = Random.Range(SphereBoundaries.attackBound.radius - 1f, SphereBoundaries.attackBound.radius - 0.05f);

        attackPosition = Utils.TravelDestinationCorrection(midPoint.normalized * SphereBoundaries.minDistanceBound.radius, player.transform.position, skull.minDistanceToGround, skull.maxDistanceToGround);
        fallBackPosition = Utils.TravelDestinationCorrection(combined.normalized * rangeBetweenRadiuses, player.transform.position, skull.minDistanceToGround, skull.maxDistanceToGround);
    }

    private class Move1 : SkullState //Quick and dirty
    {
        private SkullAttackState skullAttackState;
        public Move1(SkullAttackState skullAttackState)
        {
            this.skullAttackState = skullAttackState;
        }

        public void Action()
        {
            Vector3 orientation = (skullAttackState.player.transform.position - skullAttackState.skull.transform.position).normalized;
            orientation = skullAttackState.skull.direction == Direction.LEFT ? -orientation : orientation;
            skullAttackState.skull.SmoothLook(orientation);

            skullAttackState.skull.transform.position = Vector3.MoveTowards(skullAttackState.skull.transform.position, skullAttackState.attackPosition, skullAttackState.skull.movementSpeed * Time.deltaTime);
            if (Vector3.Distance(skullAttackState.skull.transform.position, skullAttackState.attackPosition) < 0.01f)
            {
                skullAttackState.move = new Move2(skullAttackState);
            }
        }
    }
    private class Move2 : SkullState
    {
        private SkullAttackState skullAttackState;
        private float timer;

        public Move2(SkullAttackState skullAttackState)
        {
            this.skullAttackState = skullAttackState;
            timer = 2;
        }

        public void Action()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                skullAttackState.move = new Move3(skullAttackState);
            }
        }
    }

    private class Move3 : SkullState
    {
        private SkullAttackState skullAttackState;

        public Move3(SkullAttackState skullAttackState)
        {
            this.skullAttackState = skullAttackState;
        }

        public void Action()
        {
            Vector3 orientation = (skullAttackState.player.transform.position - skullAttackState.skull.transform.position).normalized;
            orientation = skullAttackState.skull.direction == Direction.LEFT ? -orientation : orientation;
            skullAttackState.skull.SmoothLook(orientation);

            skullAttackState.skull.transform.position = Vector3.MoveTowards(skullAttackState.skull.transform.position, skullAttackState.fallBackPosition, skullAttackState.skull.movementSpeed * Time.deltaTime);

            if (Vector3.Distance(skullAttackState.skull.transform.position, skullAttackState.fallBackPosition) < 0.01f)
            {
                skullAttackState.player.GetComponent<PLayerHealth>().TakeDamage(20);
                skullAttackState.move = new Move4(skullAttackState);
            }
        }
    }

    private class Move4 : SkullState
    {
        private SkullAttackState skullAttackState;
        private float timer = 2;

        public Move4(SkullAttackState skullAttackState)
        {
            this.skullAttackState = skullAttackState;
        }

        public void Action()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                skullAttackState.startingPosition = skullAttackState.skull.transform.position;
                skullAttackState.CalculatePositions();
                skullAttackState.move = new Move1(skullAttackState);
            }
        }
    }
}
