using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SphereBoundaries : MonoBehaviour
{
    [SerializeField] private GameObject attackZone;
    [SerializeField] private GameObject minDistanceZone;
    [SerializeField] private GameObject player;

    public static Sphere attackBound { get; private set; } //TODO: Just remembered that center is the same for both, so, I could just stop using classes here or use static variable
    public static Sphere minDistanceBound { get; private set; }
    private static Vector3 playerForward;
    void Awake()
    {
        attackBound = new Sphere(attackZone.transform.position, attackZone.transform.localScale.x / 2);
        minDistanceBound = new Sphere(minDistanceZone.transform.position, minDistanceZone.transform.localScale.x / 2);
        playerForward = player.transform.forward;
    }

    public static bool IsPointWithinSphere(Sphere sphere, Vector3 point)
    {
        return Vector3.Distance(sphere.center, point) <= sphere.radius;
    }

    public static Vector3 MovePointOutsideMinDistanceZone(Vector3 point)
    {
        Vector3 fromCenterToPoint = point - attackBound.center;
        float dotProduct = Vector3.Dot(fromCenterToPoint, playerForward);

        if (dotProduct <= 0) //fail safe
        {
            return CalculateIntersection(attackBound, playerForward);
        }
        return CalculateIntersection(attackBound, fromCenterToPoint);
    }

    private static Vector3 CalculateIntersection(Sphere sphere, Vector3 direction)
    {
        return sphere.center + direction.normalized * sphere.radius;
    }
}
