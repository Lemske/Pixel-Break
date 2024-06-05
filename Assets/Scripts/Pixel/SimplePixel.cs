using UnityEngine;

public class SimplePixel : MonoBehaviour, ForceHitDetector
{
    public void HitWithForce(Vector3 force)
    {
        transform.parent.GetComponent<EnemyController>().HandleGettingHit(force, transform.localPosition);
    }
}
