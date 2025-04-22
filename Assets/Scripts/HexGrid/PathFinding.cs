using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public static class PathFinding
{
    public static List<Node> resultPath = new List<Node>();
    public static List<Node> openNodes = new List<Node>();
    public static List<Node> visitedNodes = new List<Node>();
    public static Node startNode;
    public static Node goalNode;
    public static Node currentNode;

    public static List<Node> AStar(Node start, Node end, IGrid grid)
    {
        startNode = start;
        goalNode = end;
        resultPath.Clear();
        visitedNodes.Clear();

        currentNode = startNode;
        while (currentNode != goalNode)
        {

        }
        return resultPath;
    }
    public static void GetBestNode(Node node)
    {
        foreach (var neighbor in node.neighbors)
        {

        }
    }
    public static float Heuristic(Vector2Int from, Vector2Int to)
    {
        return Vector2Int.Distance(from, to);
    }
}
