using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPointInBox(Vector3 dimensions, Vector3 center, Quaternion rotation)
    {
        Vector3 randomCubeLocation = new Vector3(Random.Range(-dimensions.x / 2f, dimensions.x / 2f), Random.Range(-dimensions.y / 2f, dimensions.y / 2f), Random.Range(-dimensions.z / 2f, dimensions.z / 2f));
        return center + (rotation * randomCubeLocation);
    }

    public static TravelBox[] GenerateTravelBoxes(Direction skullFacingDirection, Transform skull, float length, float width, float height, float minLength, float minLengthFromGround, float maxLengthFromGround)
    {
        Vector3 direction = skull.forward;
        if (skullFacingDirection == Direction.LEFT)
        {
            direction = -direction;
        }
        float lengthAway = minLength + (length / 2);
        Vector3 right = skull.right * width / 2;
        Vector3 left = -right;

        if (skullFacingDirection == Direction.LEFT)
        {
            right = -right;
            left = -left;
        }

        Vector3 rightCenterRegion = direction * lengthAway + skull.transform.position + right;
        Vector3 leftCenterRegion = direction * lengthAway + skull.transform.position + left;

        TravelBox[] travelBoxes = new TravelBox[2];
        travelBoxes[0] = new TravelBox
        {
            center = rightCenterRegion,
            rotation = Quaternion.LookRotation(direction)
        };

        travelBoxes[1] = new TravelBox
        {
            center = leftCenterRegion,
            rotation = Quaternion.LookRotation(direction)
        };

        Vector3 rightLowestVertex = GetExtremeVertex(travelBoxes[0], width, height, length);

        if (rightLowestVertex.y < minLengthFromGround)
        {
            Vector3 upDirection = skull.up;
            float yDif = minLengthFromGround - rightLowestVertex.y;
            Vector3 toAdd = upDirection * (yDif / upDirection.y);
            travelBoxes[0].center += toAdd;
            travelBoxes[1].center += toAdd;

            return travelBoxes;
        }

        Vector3 rightHighestVertex = GetExtremeVertex(travelBoxes[0], width, height, length, false);

        if (rightHighestVertex.y > maxLengthFromGround)
        {
            Vector3 upDirection = -skull.up;
            float yDif = rightHighestVertex.y - maxLengthFromGround;
            Vector3 toAdd = upDirection * (yDif / upDirection.y);
            travelBoxes[0].center -= toAdd;
            travelBoxes[1].center -= toAdd;
        }

        return travelBoxes;
    }

    private static Vector3 GetExtremeVertex(TravelBox travelBox, float width, float height, float length, bool checkLowest = true)
    {
        Vector3 center = travelBox.center;
        Quaternion rotation = travelBox.rotation;

        float halfWidth = width / 2;
        float halfLength = length / 2;
        float halfHeight = height / 2;

        halfHeight = checkLowest ? -halfHeight : halfHeight;

        Vector3 backVertex = center + rotation * new Vector3(halfWidth, halfHeight, -halfLength);
        Vector3 frontVertex = center + rotation * new Vector3(halfWidth, halfHeight, halfLength);
        if (checkLowest)
        {
            return backVertex.y < frontVertex.y ? backVertex : frontVertex;
        }

        return backVertex.y > frontVertex.y ? backVertex : frontVertex;
    }

    public static bool IsLookingAt(Vector3 position, Vector3 target, Vector3 forward, float tolerance = 1)
    {
        Vector3 direction = target - position;
        float angleBetween = Vector3.Angle(forward, direction);
        return angleBetween <= tolerance;
    }

    public static float EaseInOut(float t)
    {
        // Smoothstep-like easing function
        return t * t * (3f - 2f * t);
    }

    public static void TravelDestinationCorrection(Vector3 travelDestination, Vector3 playerPosition, float minY, float maxY)
    {
        List<Vector3>[] bounds = PlayerView.InBoundsVectors;
        for (int i = 0; i < bounds[1].Count; i++)
        {
            Vector3 fromPlayerToDestination = travelDestination - playerPosition;
            float dotProduct = Vector3.Dot(fromPlayerToDestination.normalized, bounds[1][i]);
            if (dotProduct < 0)
            { //Might need to be bigger
                Vector3 projection = Vector3.Project(fromPlayerToDestination, bounds[0][i]);
                Vector3 correctionVector = projection - fromPlayerToDestination;
                Debug.DrawLine(travelDestination, travelDestination + correctionVector, Color.red, 10);
                travelDestination = travelDestination + correctionVector + correctionVector.normalized * 2;
                travelDestination.y = travelDestination.y < minY ? minY : travelDestination.y;
                travelDestination.y = travelDestination.y > maxY ? maxY : travelDestination.y;
            }
        }
    }
}
