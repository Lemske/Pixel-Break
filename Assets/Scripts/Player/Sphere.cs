using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere
{
    public Vector3 center { get; private set; }
    public float radius { get; private set; }

    public Sphere(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}
