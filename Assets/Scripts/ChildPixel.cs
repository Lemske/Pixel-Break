using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildPixel : MonoBehaviour, ForceHitDetector
{
    public void HitWithForce(Vector3 force)
    {
        Debug.Log(transform.localPosition);
        transform.parent.GetComponent<ParentPixel>().HandleGettingHit(force, transform.localPosition);
    }
}
