using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParentPixel : MonoBehaviour, ForceHitDetector
{
    //2d Array for childrens positions compared to eachother
    private ChildPixel[,] childrenArray;
    private String[,] childrenNames;//Todo: delete this line
    private Vector2 parentPosition;
    private bool parentHit = false;
    int minX;
    int minY;
    void Start()
    {
        int maxX = 0;
        int maxY = 0;
        foreach (Transform child in transform)
        {
            Vector3 localPosition = child.localPosition;
            if (localPosition.x < minX)
            {
                minX = (int)localPosition.x;
            }
            else if (localPosition.x > maxX)
            {
                maxX = (int)localPosition.x;
            }
            if (localPosition.y < minY)
            {
                minY = (int)localPosition.y;
            }
            else if (localPosition.y > maxY)
            {
                maxY = (int)localPosition.y;
            }
            child.gameObject.AddComponent<ChildPixel>();
        }
        childrenArray = new ChildPixel[maxY - minY + 1, maxX - minX + 1];
        childrenNames = new String[maxY - minY + 1, maxX - minX + 1];//Todo: delete this line

        foreach (Transform child in transform)
        {
            Vector3 localPosition = child.localPosition;
            int y = (int)localPosition.y - minY; // y represents the row
            int x = (int)localPosition.x - minX; // x represents the column

            childrenArray[y, x] = child.GetComponent<ChildPixel>();
            childrenNames[y, x] = child.name; //Todo: delete this line
        }
        parentPosition = new Vector2(0 - minX, 0 - minY);
        PrintChildrenArray();
    }

    public void ChildHit(String childName) //Todo: Is this still in use?
    {
        Debug.Log("Child " + childName + " was hit!");
    }

    public void ParentHandleChildHit(Vector3 force, Vector2 localPosition)
    {
        int y = (int)localPosition.y - minY;
        int x = (int)localPosition.x - minX;
        Vector2 newLocalPosition = new Vector2(x, y);

        List<GameObject> surroundingChildren = GetSurroundingChildren(newLocalPosition, 1, force);
        foreach (GameObject child in surroundingChildren)
        {
            child.transform.parent = null;
            child.AddComponent<Rigidbody>().AddForce(force * 10, ForceMode.Impulse); //Todo: make force a parameter
        }

        if (parentHit)
        {
            transform.AddComponent<Rigidbody>().AddForce(force * 10, ForceMode.Impulse);
            foreach (ChildPixel child in childrenArray)
            {
                if (child != null)
                {
                    child.transform.parent = null;
                    child.gameObject.AddComponent<Rigidbody>();
                }
            }
        }

        ChildPixel[,] connectedChildren = StillConnectedToParentPosition(parentPosition); //Todo: This and down can be done better i think
        //remove children not connected to parent
        for (int i = 0; i < childrenArray.GetLength(0); i++)
        {
            for (int j = 0; j < childrenArray.GetLength(1); j++)
            {
                if (connectedChildren[i, j] == null && childrenArray[i, j] != null)
                {
                    childrenArray[i, j].transform.parent = null;
                    childrenArray[i, j].gameObject.AddComponent<Rigidbody>();
                }
            }
        }
    }

    private List<GameObject> GetSurroundingChildren(Vector2 localPosition, int radius, Vector3 force)
    {
        List<GameObject> surroundingChildren = new List<GameObject>();
        int xDirection = force.x > 0.5 ? 1 : force.x < -0.5 ? -1 : 0;
        int yDirection = force.y > 0.5 ? 1 : force.y < -0.5 ? -1 : 0;
        Vector2 newLocalPosition = localPosition + new Vector2(xDirection, yDirection);
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int y = (int)newLocalPosition.y + i;
                int x = (int)newLocalPosition.x + j;
                bool withinXBounds = x >= 0 && x < childrenArray.GetLength(1);
                bool withinYBounds = y >= 0 && y < childrenArray.GetLength(0);
                if (!withinXBounds || !withinYBounds)
                {
                    continue;
                }
                if (childrenArray[y, x] != null)
                {
                    surroundingChildren.Add(childrenArray[y, x].gameObject);
                    childrenArray[y, x] = null;
                }
                else if (x == parentPosition.x && y == parentPosition.y)
                {
                    parentHit = true;
                }
            }
        }
        return surroundingChildren;
    }

    private ChildPixel[,] StillConnectedToParentPosition(Vector2 parentPosition)
    {
        ChildPixel[,] connectedChildren = new ChildPixel[childrenArray.GetLength(0), childrenArray.GetLength(1)];
        Queue<(int, int)> queue = new Queue<(int, int)>();
        connectedChildren[(int)parentPosition.y, (int)parentPosition.x] = childrenArray[(int)parentPosition.y, (int)parentPosition.x];
        queue.Enqueue(((int)parentPosition.y, (int)parentPosition.x));
        while (queue.Count > 0)
        {
            //Check neighbours vertically and horizontally
            (int, int) current = queue.Dequeue();
            int y = current.Item1;
            int x = current.Item2;
            if (y - 1 >= 0 && childrenArray[y - 1, x] != null && connectedChildren[y - 1, x] == null) //Doing it the lazy way
            {
                connectedChildren[y - 1, x] = childrenArray[y - 1, x];
                queue.Enqueue((y - 1, x));
            }
            if (y + 1 < childrenArray.GetLength(0) && childrenArray[y + 1, x] != null && connectedChildren[y + 1, x] == null)
            {
                connectedChildren[y + 1, x] = childrenArray[y + 1, x];
                queue.Enqueue((y + 1, x));
            }
            if (x - 1 >= 0 && childrenArray[y, x - 1] != null && connectedChildren[y, x - 1] == null)
            {
                connectedChildren[y, x - 1] = childrenArray[y, x - 1];
                queue.Enqueue((y, x - 1));
            }
            if (x + 1 < childrenArray.GetLength(1) && childrenArray[y, x + 1] != null && connectedChildren[y, x + 1] == null)
            {
                connectedChildren[y, x + 1] = childrenArray[y, x + 1];
                queue.Enqueue((y, x + 1));
            }
        }
        return connectedChildren;
    }

    private void PrintChildrenArray()//Todo: delete this method
    {
        for (int i = 0; i < childrenArray.GetLength(0); i++)
        {
            for (int j = 0; j < childrenArray.GetLength(1); j++)
            {
                if (childrenArray[i, j] != null)
                {
                    Debug.Log("Child at position (" + i + ", " + j + "): " + childrenNames[i, j]);
                }
                else
                {
                    Debug.Log("No child at position (" + i + ", " + j + ")");
                }
            }
        }
    }

    public void HitWithForce(Vector3 force)
    {
        ParentHandleChildHit(force, new Vector2(0, 0)); //Have to set it to 0, 0 since this method changes the position to fit the array
    }
}
