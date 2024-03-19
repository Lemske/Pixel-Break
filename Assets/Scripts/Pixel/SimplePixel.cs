using UnityEngine;

public class SimplePixel : MonoBehaviour, ForceHitDetector
{
    public void HitWithForce(Vector3 force)
    {
        Debug.Log(transform.localPosition);
        transform.parent.GetComponent<CorePixel>().HandleGettingHit(force, transform.localPosition);
    }
}
