using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathFinding
{
    public static List<Node> resultPath = new List<Node>();
    private static List<Node> openNodes = new List<Node>();
    private static List<Node> visitedNodes = new List<Node>();
    public static List<Node> Result
    {
        get
        {
            return resultPath;
        }
    }

    public static void AStar(Node start, Node end)
    {
        start.gCost = 0;
        start.hCost = Heuristic(start, end);

        resultPath.Clear();
        openNodes.Clear();
        visitedNodes.Clear();

        Node currentNode = start;
        openNodes.Add(currentNode);
        while (openNodes.Count > 0)
        {
            currentNode = openNodes.OrderBy(n => n.fCost).ThenBy(n => n.hCost).First();
            //Debug.LogError("Take: " + currentNode.Position, currentNode.NodeGO);
            if (currentNode == end)
            {
                TrackingBackPath(start, end);
                return;
            }
            openNodes.Remove(currentNode);
            visitedNodes.Add(currentNode);
            //Debug.LogError("Neighbours: " + currentNode.neighbors.Count, currentNode.NodeGO);
            foreach (var neighbor in currentNode.neighbors)
            {
                if (visitedNodes.Contains(neighbor))
                {
                    continue;
                }
                float estimatedCost = currentNode.gCost + neighbor.mCost;
                if (!openNodes.Contains(neighbor) || estimatedCost < neighbor.gCost)
                {
                    neighbor.gCost = estimatedCost;
                    neighbor.hCost = Heuristic(neighbor, end);
                    neighbor.prevNode = currentNode;
                    if (!openNodes.Contains(neighbor))
                    {
                        //Debug.LogError("Add: " + neighbor.Position, neighbor.NodeGO);
                        openNodes.Add(neighbor);
                    }
                }
            }
        }
    }
    public static List<Node> TrackingBackPath(Node start, Node end)
    {
        resultPath.Clear();
        Node cur = end.prevNode;
        while (cur != start)
        {
            resultPath.Add(cur);
            cur = cur.prevNode;
        }
        resultPath.Reverse();
        return resultPath;
    }
    public static float Heuristic(Node from, Node to)
    {
        return Vector2Int.Distance(from.Position, to.Position);
    }
}
