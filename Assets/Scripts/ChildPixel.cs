using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildPixel : MonoBehaviour, ForceHitDetector
{
    public void HitWithForce(Vector3 force)
    {
        //Debug.Log(gameObject.name + " hit with force: " + force);
        transform.parent.GetComponent<ParentPixel>().HandleGettingHit(force, transform.localPosition);
    }
}
