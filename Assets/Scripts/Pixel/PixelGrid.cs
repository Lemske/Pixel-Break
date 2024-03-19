using System.Collections.Generic;
using UnityEngine;

public class PixelGrid
{
    public Pixel[,] grid;
    public int xWidth;
    public int yHeight;
    public int minXOffset;
    public int minYOffset;
    public int[] corePositionXY;

    public PixelGrid(Transform corePixel)
    {
        InitializeGrid(corePixel);
        PopulateGrid(corePixel);
        xWidth = grid.GetLength(1);
        yHeight = grid.GetLength(0);
        corePositionXY = new int[] { 0 - minXOffset, 0 - minYOffset };
    }

    public PixelGrid(int xWidth, int yHeight)
    {
        grid = new Pixel[yHeight, xWidth];
        this.xWidth = xWidth;
        this.yHeight = yHeight;
    }

    private void PopulateGrid(Transform corePixel)
    {
        foreach (Transform child in corePixel)
        {
            Vector3 localPosition = child.localPosition;
            int x = (int)localPosition.x - minXOffset;
            int y = (int)localPosition.y - minYOffset;
            grid[y, x] = new Pixel(x, y, child.gameObject);
        }
    }

    private void InitializeGrid(Transform corePixel)
    {
        int maxX = 0;
        int maxY = 0;
        foreach (Transform child in corePixel)
        {
            Vector2 localPosition = child.localPosition;
            minXOffset = (int)Mathf.Min(minXOffset, localPosition.x);
            maxX = (int)Mathf.Max(maxX, localPosition.x);
            minYOffset = (int)Mathf.Min(minYOffset, localPosition.y);
            maxY = (int)Mathf.Max(maxY, localPosition.y);

            child.gameObject.AddComponent<ChildPixel>();
        }
        grid = new Pixel[maxY - minYOffset + 1, maxX - minXOffset + 1];
    }

    public Pixel GetPixelFromLocalPosition(Vector2 localPosition)
    {
        return grid[(int)localPosition.y - minYOffset, (int)localPosition.x - minXOffset];
    }

    public Pixel GetPixelGridPosition(int x, int y)
    {
        if (x < 0 || x >= xWidth || y < 0 || y >= yHeight)
        {
            return null;
        }
        return grid[y, x];
    }

    public void RemovePixel(Pixel pixel)
    {
        grid[pixel.y, pixel.x] = null;
    }

    public void AddPixel(Pixel pixel)
    {
        if (grid[pixel.y, pixel.x] != null)
        {
            return;
        }
        grid[pixel.y, pixel.x] = pixel;
    }

    public List<Pixel> GetDifference(PixelGrid otherGrid)
    {
        List<Pixel> difference = new List<Pixel>();
        for (int y = 0; y < yHeight; y++)
        {
            for (int x = 0; x < xWidth; x++)
            {
                if (grid[y, x] != otherGrid.grid[y, x])
                {
                    difference.Add(grid[y, x]);
                }
            }
        }
        return difference;
    }

    public int[] GetXYPositionFromVector2(Vector2 position)
    {
        return new int[] { (int)position.x - minXOffset, (int)position.y - minYOffset };
    }
}
