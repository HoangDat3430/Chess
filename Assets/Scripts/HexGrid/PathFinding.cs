using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PathFinding
{
    public static List<Node> resultPath = new List<Node>();
    public static List<Node> frontierNodes = new List<Node>();
    public static List<Node> visitedNodes = new List<Node>();
    public static Node startNode;
    public static Node goalNode;
    public static Node currentNode;
    
    public static bool AStar(GameObject start, GameObject end, GameObject[,] gridMap)
    {
        if(startNode == null)
        {
            startNode = new Node(start);
        }
        else
        {
            startNode.NodeGO = start;
        }
        if (goalNode == null)
        {
            goalNode = new Node(end);
        }
        else
        {
            goalNode.NodeGO = end;
        }
        return false;
    }
}
