using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildPixel : MonoBehaviour, ForceHitDetector
{
    void OnCollisionEnter(Collision collision) //Todo: Is this still in use?
    {
        //transform.parent.GetComponent<ParentPixel>().ChildHit(gameObject.name);
    }

    public void HitWithForce(Vector3 force)
    {
        //Debug.Log(gameObject.name + " hit with force: " + force);
        transform.parent.GetComponent<ParentPixel>().ParentHandleChildHit(force, transform.localPosition);
    }
}
