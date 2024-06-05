using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelController : MonoBehaviour
{
    [Header("Horizontal/Vertical Thresholds")]
    [SerializeField, Range(0, 1)] private float vDirThreshold = 0.7f;
    [SerializeField, Range(0, 1)] private float hDirThreshold = 0.7f;
    [SerializeField, Range(0, 1)] private float vClearThreshold = 0.9f;
    [SerializeField, Range(0, 1)] private float hClearThreshold = 0.9f;
    public PixelGrid pixelGrid { get; private set; }

    public void Initialize(GameObject pixelParent)
    {
        pixelGrid = new PixelGrid(pixelParent.transform);
    }

    public List<Pixel> FindPixelsWithinRadius(int x, int y, int radius, Vector3 force, GameObject pixelParent)
    {
        float rightProduct = Vector3.Dot(force, pixelParent.transform.right);
        float upProduct = Vector3.Dot(force, pixelParent.transform.up);

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
                    return new List<Pixel>(); //Returns an empty list if the core pixel is hit
                }
            }
        }
        if (rightProduct > hClearThreshold || rightProduct < -hClearThreshold)
        {
            for (int i = 0; i < pixelGrid.xWidth; i++)
            {
                Pixel pixel = pixelGrid.GetPixelGridPosition(i, y);
                if (pixel != null && !surroundingPixels.Contains(pixel))
                {
                    surroundingPixels.Add(pixel);
                }
                if (i == pixelGrid.corePositionXY[0] && y == pixelGrid.corePositionXY[1])
                {
                    return new List<Pixel>(); //Returns an empty list if the core pixel is hit
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
                    return new List<Pixel>(); //Returns an empty list if the core pixel is hit
                }
            }
        }
        return surroundingPixels;
    }

    public List<Pixel> GetAllRemainingPixelsPositions()
    {
        List<Pixel> remainingPixels = new List<Pixel>();
        for (int i = 0; i < pixelGrid.xWidth; i++)
        {
            for (int j = 0; j < pixelGrid.yHeight; j++)
            {
                Pixel pixel = pixelGrid.GetPixelGridPosition(i, j);
                if (pixel != null)
                {
                    remainingPixels.Add(pixel);
                }
            }
        }
        return remainingPixels;
    }

    public PixelGrid FindConnectedPixels()
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

    public List<Pixel> DetectDisconnectedChildren()
    {
        PixelGrid stillConnectedToParent = FindConnectedPixels();
        return pixelGrid.GetDifference(stillConnectedToParent);
    }

    public int[] GetPixelCoordinates(Vector2 localPosition)
    {
        return pixelGrid.GetXYPositionFromVector2(localPosition);
    }

    public void RemovePixels(List<Pixel> pixels)
    {
        foreach (Pixel pixel in pixels)
        {
            pixelGrid.RemovePixel(pixel);
        }
    }

    public int[] GetCorePosition()
    {
        return pixelGrid.corePositionXY;
    }
}
