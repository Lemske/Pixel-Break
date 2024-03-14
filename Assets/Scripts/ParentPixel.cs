using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParentPixel : MonoBehaviour, ForceHitDetector
{
    //2d Array for childrens positions compared to eachother
    private GameObject[,] childGrid;
    private Vector2 parentPosition;
    private bool parentHit = false;
    int minX;
    int minY;
    [Header("Points and Scoring")]
    [SerializeField] private int points = 100;
    [SerializeField] private int pointsDecayPerHitPercent = 10;
    private ScoreCanvasController scoreCanvasController;
    private int timesHit = 0;

    void Start()
    {
        InitChildrenGrid();

        foreach (Transform child in transform)
        {
            Vector3 localPosition = child.localPosition;
            int y = (int)localPosition.y - minY; // y represents the row
            int x = (int)localPosition.x - minX; // x represents the column

            childGrid[y, x] = child.gameObject;
        }
        parentPosition = new Vector2(0 - minX, 0 - minY);
        scoreCanvasController = FindObjectOfType<ScoreCanvasController>();
    }

    private void InitChildrenGrid() //TODO: This code should be used on all prefabs, so It can be set in the prefab
    {
        int maxX = 0;
        int maxY = 0;
        foreach (Transform child in transform)
        {
            Vector3 localPosition = child.localPosition;
            minX = (int)Mathf.Min(minX, localPosition.x);
            maxX = (int)Mathf.Max(maxX, localPosition.x);
            minY = (int)Mathf.Min(minY, localPosition.y);
            maxY = (int)Mathf.Max(maxY, localPosition.y);

            child.gameObject.AddComponent<ChildPixel>();
        }
        childGrid = new GameObject[maxY - minY + 1, maxX - minX + 1];
    }

    public void HandleGettingHit(Vector3 force, Vector2 localPosition)
    {
        int y = (int)localPosition.y - minY;
        int x = (int)localPosition.x - minX;
        Vector2 newLocalPosition = new Vector2(x, y);

        List<Vector2> surroundingChildren = FindHitChildrenLocations(newLocalPosition, 1, force);
        AddForceToChildren(surroundingChildren, force, newLocalPosition);

        if (parentHit)
        {
            transform.AddComponent<Rigidbody>().AddForce(force * 10, ForceMode.Impulse);
            AddForceToChildren(AllRemainingChildrenPositions(), force, newLocalPosition);
            scoreCanvasController.AddScore(Mathf.RoundToInt(points * Mathf.Pow(1 - pointsDecayPerHitPercent / 100f, timesHit)), ScoreCanvasController.ScoreType.Normal);
            SimpleMovement simpleMovement = transform.GetComponent<SimpleMovement>(); //TODO: Don't like this way of doing it
            if (simpleMovement != null)
            {
                simpleMovement.enabled = false;
            }
            return;
        }

        List<Vector2> disconnectedChildren = DetectDisconnectedChildren();
        AddForceToChildren(disconnectedChildren, force / 2, newLocalPosition); //feels weird with full force
        timesHit++;
    }

    private List<Vector2> AllRemainingChildrenPositions()
    {
        List<Vector2> remainingChildren = new List<Vector2>();
        for (int i = 0; i < childGrid.GetLength(0); i++)
        {
            for (int j = 0; j < childGrid.GetLength(1); j++)
            {
                if (childGrid[i, j] != null)
                {
                    remainingChildren.Add(new Vector2(j, i));
                }
            }
        }
        return remainingChildren;
    }

    private List<Vector2> FindHitChildrenLocations(Vector2 localPosition, int radius, Vector3 force)
    {
        //Calculating the direction of the force onto the object
        float dotProduct = Vector3.Dot(force, transform.forward);
        if (dotProduct == 0) //Removing edge case where its directly 0, can be fixed with doing some different math, but cant think of it right now
        {
            parentHit = true;
            return new List<Vector2>();
        }
        bool perpendicular = dotProduct > -0.2 && dotProduct < 0.2; //TODO: The 0.2 should be a parameter
        float rightProduct = Vector3.Dot(force, transform.right);
        float upProduct = Vector3.Dot(force, transform.up);
        Debug.Log("Right: " + rightProduct + " Up: " + upProduct);

        List<Vector2> surroundingChildren = new List<Vector2>();
        int column = rightProduct > 0.7 ? 1 : rightProduct < -0.7 ? -1 : 0;
        int row = upProduct > 0.7 ? 1 : upProduct < -0.7 ? -1 : 0;
        Vector2 newLocalPosition = localPosition + new Vector2(column, row);

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                int y = (int)newLocalPosition.y + i;
                int x = (int)newLocalPosition.x + j;
                bool withinXBounds = x >= 0 && x < childGrid.GetLength(1);
                bool withinYBounds = y >= 0 && y < childGrid.GetLength(0);
                if (!withinXBounds || !withinYBounds)
                {
                    continue;
                }
                if (childGrid[y, x] != null)
                {
                    surroundingChildren.Add(new Vector2(x, y));
                }
                else if (x == parentPosition.x && y == parentPosition.y)
                {
                    parentHit = true;
                }
            }
        }
        if (perpendicular)
        {
            if (rightProduct > 0.7 || rightProduct < -0.7)
            {
                for (int i = 0; i < childGrid.GetLength(1); i++)
                {
                    Vector2 check = new Vector2(i, localPosition.y);
                    if (childGrid[(int)localPosition.y, i] != null && !surroundingChildren.Contains(check))
                    {
                        if (i == parentPosition.x && localPosition.y == parentPosition.y)
                        {
                            parentHit = true;
                        }
                        surroundingChildren.Add(check);
                    }
                }
            }
            if (upProduct > 0.7 || upProduct < -0.7)
            {
                for (int i = 0; i < childGrid.GetLength(0); i++)
                {
                    Vector2 check = new Vector2(localPosition.x, i);
                    if (i == parentPosition.y && localPosition.x == parentPosition.x)
                    {
                        parentHit = true;
                    }
                    if (childGrid[i, (int)localPosition.x] != null && !surroundingChildren.Contains(check))
                    {
                        surroundingChildren.Add(check);
                    }
                }
            }
        }

        return surroundingChildren;
    }

    private void AddForceToChildren(List<Vector2> childrenHit, Vector3 force, Vector2 locationHit)
    {
        foreach (Vector2 childPosition in childrenHit)
        {
            GameObject child = childGrid[(int)childPosition.y, (int)childPosition.x];
            child.transform.parent = null;
            childGrid[(int)childPosition.y, (int)childPosition.x] = null;
            float distance = Vector2.Distance(locationHit, childPosition);
            float forceMultiplier = distance == 0 ? 10 : 10 / distance * 2; //Todo: The 2 should be a parameter
            child.AddComponent<Rigidbody>().AddForce(force * forceMultiplier, ForceMode.Impulse);
            Destroy(child.GetComponent<ChildPixel>());
        }
    }

    private GameObject[,] FindConnectedChildren() //Todo: revisit later, might be a better way
    {
        GameObject[,] connectedChildren = new GameObject[childGrid.GetLength(0), childGrid.GetLength(1)];
        Queue<(int, int)> queue = new Queue<(int, int)>();
        connectedChildren[(int)parentPosition.y, (int)parentPosition.x] = childGrid[(int)parentPosition.y, (int)parentPosition.x];
        queue.Enqueue(((int)parentPosition.y, (int)parentPosition.x));
        while (queue.Count > 0)
        {
            //Check neighbours vertically and horizontally
            (int, int) current = queue.Dequeue();
            int y = current.Item1;
            int x = current.Item2;
            if (y - 1 >= 0 && childGrid[y - 1, x] != null && connectedChildren[y - 1, x] == null) //Doing it the lazy way, many if statements
            {
                connectedChildren[y - 1, x] = childGrid[y - 1, x];
                queue.Enqueue((y - 1, x));
            }
            if (y + 1 < childGrid.GetLength(0) && childGrid[y + 1, x] != null && connectedChildren[y + 1, x] == null)
            {
                connectedChildren[y + 1, x] = childGrid[y + 1, x];
                queue.Enqueue((y + 1, x));
            }
            if (x - 1 >= 0 && childGrid[y, x - 1] != null && connectedChildren[y, x - 1] == null)
            {
                connectedChildren[y, x - 1] = childGrid[y, x - 1];
                queue.Enqueue((y, x - 1));
            }
            if (x + 1 < childGrid.GetLength(1) && childGrid[y, x + 1] != null && connectedChildren[y, x + 1] == null)
            {
                connectedChildren[y, x + 1] = childGrid[y, x + 1];
                queue.Enqueue((y, x + 1));
            }
        }
        return connectedChildren;
    }

    private List<Vector2> DetectDisconnectedChildren() //Todo: revisit later, might be a better way
    {
        List<Vector2> disconnectedChildren = new List<Vector2>();
        GameObject[,] stillConnectedToParent = FindConnectedChildren();

        for (int i = 0; i < childGrid.GetLength(0); i++)
        {
            for (int j = 0; j < childGrid.GetLength(1); j++)
            {
                if (stillConnectedToParent[i, j] == null && childGrid[i, j] != null)
                {
                    disconnectedChildren.Add(new Vector2(j, i));
                }
            }
        }

        return disconnectedChildren;
    }

    public void HitWithForce(Vector3 force)
    {
        if (!parentHit)
            HandleGettingHit(force, new Vector2(0, 0)); //Have to set it to 0, 0 since handle getting hit calc array position
    }
}
