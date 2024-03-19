using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CorePixel : MonoBehaviour, ForceHitDetector
{
    private PixelGrid pixelGrid;
    private ScoreCanvasController scoreCanvasController;
    private int hitCount = 0;
    [Header("Horizontal/Vertical shoot")]
    [SerializeField, Range(0, 1)] private float vDirThreshold = 0.7f;
    [SerializeField, Range(0, 1)] private float hDirThreshold = 0.7f;
    [SerializeField, Range(0, 1)] private float vClearThreshold = 0.9f;
    [SerializeField, Range(0, 1)] private float hClearThreshold = 0.9f;
    [Header("Force Appliers")]
    [SerializeField] private float distanceMultiplier = 2;
    [SerializeField] private float maxForce = 10;
    [SerializeField] private float disconnectedReducer = 2;
    [Header("Points and Scoring")]
    [SerializeField] private int points = 100;
    [SerializeField] private int pointsDecayPercentage = 10;


    void Start()
    {
        pixelGrid = new PixelGrid(transform);
        scoreCanvasController = FindObjectOfType<ScoreCanvasController>();
    }

    public void HandleGettingHit(Vector3 force, Vector2 localPosition)
    {
        int[] gridPosition = pixelGrid.GetXYPositionFromVector2(localPosition);
        List<Pixel> surroundingPixels = FindHitPixelsLocations(gridPosition[0], gridPosition[1], 1, force);

        if (surroundingPixels.Count == 0)
            return;

        Vector2 localPositionVector = new Vector2(gridPosition[0], gridPosition[1]);
        ApplyForceToPixels(surroundingPixels, force, localPositionVector);

        List<Pixel> disconnectedChildren = DetectDisconnectedChildren();
        ApplyForceToPixels(disconnectedChildren, force / disconnectedReducer, localPositionVector);
        hitCount++;
    }

    private List<Pixel> GetAllRemainingPixelsPositions()
    {
        List<Pixel> remainingChildren = new List<Pixel>();
        for (int y = 0; y < pixelGrid.yHeight; y++)
        {
            for (int x = 0; x < pixelGrid.xWidth; x++)
            {
                Pixel pixel = pixelGrid.GetPixelGridPosition(x, y);
                if (pixel != null)
                {
                    remainingChildren.Add(pixel);
                }
            }
        }
        return remainingChildren;
    }

    private List<Pixel> FindHitPixelsLocations(int x, int y, int radius, Vector3 force) //Might someday want another spread effect than radius
    {
        float rightProduct = Vector3.Dot(force, transform.right);
        float upProduct = Vector3.Dot(force, transform.up);

        List<Pixel> surroundingPixels = new List<Pixel>();

        int xDir = rightProduct > hDirThreshold ? 1 : rightProduct < -hDirThreshold ? -1 : 0;
        int yDir = upProduct > vDirThreshold ? 1 : upProduct < -vDirThreshold ? -1 : 0;
        int newX = x + xDir;
        int newY = y + yDir;

        int startX = Mathf.Max(0, newX - radius);
        int endX = Mathf.Min(pixelGrid.xWidth - 1, newX + radius);
        int startY = Mathf.Max(0, newY - radius);
        int endY = Mathf.Min(pixelGrid.yHeight - 1, newY + radius);

        for (int yi = startY; yi <= endY; yi++)
        {
            for (int xi = startX; xi <= endX; xi++)
            {
                Pixel pixel = pixelGrid.GetPixelGridPosition(xi, yi);
                if (pixel != null)
                {
                    surroundingPixels.Add(pixel);
                }
                else if (xi == pixelGrid.corePositionXY[0] && yi == pixelGrid.corePositionXY[1])
                {
                    return CoreHit(force, x, y);
                }
            }
        }
        if (rightProduct > hClearThreshold || rightProduct < -hClearThreshold)
        {
            for (int i = 0; i < pixelGrid.xWidth; i++)
            {
                Debug.Log("i: " + i + " y: " + y);
                Pixel pixel = pixelGrid.GetPixelGridPosition(i, y);
                if (pixel != null && !surroundingPixels.Contains(pixel))
                {
                    surroundingPixels.Add(pixel);
                }
                if (i == pixelGrid.corePositionXY[0] && y == pixelGrid.corePositionXY[1])
                {
                    return CoreHit(force, x, y);
                }
            }
        }
        if (upProduct > vClearThreshold || upProduct < -vClearThreshold)
        {
            for (int i = 0; i < pixelGrid.yHeight; i++)
            {
                Pixel pixel = pixelGrid.GetPixelGridPosition(x, i);
                if (pixel != null && !surroundingPixels.Contains(pixel))
                {
                    surroundingPixels.Add(pixel);
                }
                else if (x == pixelGrid.corePositionXY[0] && i == pixelGrid.corePositionXY[1])
                {
                    return CoreHit(force, x, y);
                }
            }
        }
        return surroundingPixels;
    }

    private void ApplyForceToPixels(List<Pixel> childrenHit, Vector3 force, Vector2 locationHit)
    {
        foreach (Pixel child in childrenHit)
        {
            child.pixel.transform.parent = null;
            pixelGrid.RemovePixel(child);
            float distance = Vector2.Distance(locationHit, new Vector2(child.x, child.y));
            float forceMultiplier = distance == 0 ? maxForce : maxForce / distance * distanceMultiplier;
            GameObject pixel = child.pixel;
            pixel.AddComponent<Rigidbody>().AddForce(force * forceMultiplier, ForceMode.Impulse);
            Destroy(pixel.GetComponent<SimplePixel>());
        }
    }

    private PixelGrid FindConnectedPixels()
    {
        PixelGrid connectedChildren = new PixelGrid(pixelGrid.xWidth, pixelGrid.yHeight);
        Queue<(int, int)> queue = new Queue<(int, int)>();
        queue.Enqueue((pixelGrid.corePositionXY[1], pixelGrid.corePositionXY[0]));
        while (queue.Count > 0) //Check neighbours vertically and horizontally
        {
            (int y, int x) = queue.Dequeue();
            EnqueueNeighbors(x, y - 1, connectedChildren, queue);
            EnqueueNeighbors(x, y + 1, connectedChildren, queue);
            EnqueueNeighbors(x - 1, y, connectedChildren, queue);
            EnqueueNeighbors(x + 1, y, connectedChildren, queue);
        }
        return connectedChildren;
    }

    private void EnqueueNeighbors(int x, int y, PixelGrid connectedChildren, Queue<(int, int)> queue)
    {
        Pixel pixel = pixelGrid.GetPixelGridPosition(x, y);
        if (pixel != null && connectedChildren.GetPixelGridPosition(x, y) == null)
        {
            connectedChildren.AddPixel(pixel);
            queue.Enqueue((y, x));
        }
    }

    private List<Pixel> DetectDisconnectedChildren()
    {
        PixelGrid stillConnectedToParent = FindConnectedPixels();
        return pixelGrid.GetDifference(stillConnectedToParent);
    }

    private List<Pixel> CoreHit(Vector3 force, int xHitPos, int yHitPos)
    {
        transform.AddComponent<Rigidbody>().AddForce(force * maxForce, ForceMode.Impulse);
        ApplyForceToPixels(GetAllRemainingPixelsPositions(), force, new Vector2(xHitPos, yHitPos));
        scoreCanvasController.AddScore(Mathf.RoundToInt(points * Mathf.Pow(1 - pointsDecayPercentage / 100f, hitCount)), ScoreCanvasController.ScoreType.Normal);
        SimpleMovement simpleMovement = transform.GetComponent<SimpleMovement>();
        if (simpleMovement != null)
        {
            simpleMovement.enabled = false;
        }
        Destroy(transform.GetComponent<CorePixel>());
        return new List<Pixel>();
    }

    public void HitWithForce(Vector3 force)
    {
        int[] position = pixelGrid.corePositionXY;
        CoreHit(force, position[0], position[1]);
    }
}