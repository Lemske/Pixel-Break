using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour, ForceHitDetector
{
    [SerializeField] private PixelController pixels = new PixelController();
    [SerializeField] private PixelForceHandler forceHandler = new PixelForceHandler();
    private ScoreCanvasController scoreCanvasController;
    private int hitCount = 0;
    //TODO: Should be own things
    [Header("Aiming Field")]
    [SerializeField] private float aimHelperRadius = 0.5f;
    [SerializeField] private GameObject aimingSphere;
    [Header("Points and Scoring")]
    [SerializeField] private int points = 100;
    [SerializeField] private int pointsDecayPercentage = 10;
    //TODO: Should be own things
    void Start()
    {
        pixels.Initialize(gameObject);
        scoreCanvasController = FindObjectOfType<ScoreCanvasController>();
        GameObject sphere = Instantiate(aimingSphere);
        sphere.transform.parent = transform;
        sphere.transform.localPosition = Vector3.zero;
        sphere.GetComponent<SphereCollider>().radius = aimHelperRadius;
    }

    public void HitWithForce(Vector3 force)
    {
        int[] position = pixels.GetCorePosition();
        Death(force, position[0], position[1]);
    }

    public void HandleGettingHit(Vector3 force, Vector2 localPosition)
    {
        int[] gridPosition = pixels.GetPixelCoordinates(localPosition);

        List<Pixel> surroundingPixels = pixels.FindPixelsWithinRadius(gridPosition[0], gridPosition[1], 1, force, gameObject);
        pixels.RemovePixels(surroundingPixels);
        RemoveAllScriptsExceptRigidbody(surroundingPixels);

        if (surroundingPixels.Count == 0)
        {
            Death(force, gridPosition[0], gridPosition[1]);
            return;
        }

        Vector2 localPositionVector = new Vector2(gridPosition[0], gridPosition[1]);
        forceHandler.ApplyForceToPixels(surroundingPixels, force, localPositionVector);

        List<Pixel> disconnectedChildren = pixels.DetectDisconnectedChildren();
        forceHandler.ApplyForceToPixels(disconnectedChildren, force / forceHandler.disconnectedReducer, localPositionVector);

        pixels.RemovePixels(disconnectedChildren);

        hitCount++;
    }

    private void Death(Vector3 force, int x, int y)
    {
        forceHandler.AddMaxForce(force, gameObject);
        List<Pixel> remainingChildren = pixels.GetAllRemainingPixelsPositions();
        forceHandler.ApplyForceToPixels(remainingChildren, force, new Vector2(x, y));
        RemoveAllScriptsExceptRigidbody(remainingChildren);
        scoreCanvasController.AddScore(Mathf.RoundToInt(points * Mathf.Pow(1 - pointsDecayPercentage / 100f, hitCount)), ScoreCanvasController.ScoreType.Normal);
        RemoveAllScriptsExceptRigidbody(gameObject);
    }

    private void RemoveAllScriptsExceptRigidbody(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            RemoveAllScriptsExceptRigidbody(pixel.pixel);
        }
    }

    private void RemoveAllScriptsExceptRigidbody(GameObject obj)
    {
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType() != typeof(Transform) && script.GetType() != typeof(Rigidbody))
            {
                Destroy(script);
            }
        }
    }

    void OnDrawGizmos()
    {
        Color red = Color.red;
        Gizmos.color = red;
        Gizmos.DrawWireSphere(transform.position, aimHelperRadius);
        Gizmos.color = red * 0.3f;
        Gizmos.DrawSphere(transform.position, aimHelperRadius);
    }
}
