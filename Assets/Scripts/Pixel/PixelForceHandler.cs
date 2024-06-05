using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelForceHandler : MonoBehaviour
{
    [Header("Force Appliers")]
    [SerializeField] private float distanceMultiplier = 2;
    [SerializeField] private float maxForce = 10;
    [SerializeField] public float disconnectedReducer = 2;

    public void ApplyForceToPixels(List<Pixel> pixels, Vector3 force, Vector2 localPosition)
    {
        foreach (Pixel pixel in pixels)
        {
            pixel.pixel.transform.parent = null;
            float distance = Vector2.Distance(localPosition, new Vector2(pixel.x, pixel.y));
            float forceMultiplier = distance == 0 ? maxForce : maxForce / distance * distanceMultiplier;
            GameObject pixelObj = pixel.pixel;
            pixelObj.AddComponent<Rigidbody>().AddForce(force * forceMultiplier, ForceMode.Impulse);
        }
    }

    public void AddMaxForce(Vector3 force, GameObject pixel)
    {
        pixel.AddComponent<Rigidbody>().AddForce(force * maxForce, ForceMode.Impulse);
    }
}
