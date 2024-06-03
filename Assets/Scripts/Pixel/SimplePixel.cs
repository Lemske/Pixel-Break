using UnityEngine;

public class SimplePixel : MonoBehaviour, ForceHitDetector
{
    public void HitWithForce(Vector3 force)
    {
        transform.parent.GetComponent<CorePixel>().HandleGettingHit(force, transform.localPosition);
    }
}
