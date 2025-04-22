using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Node
{
    private GameObject nodeGO;
    private Vector2Int pos;
    private float gCost = float.MaxValue; // cost so far
    private float hCost; // heuristic
    private float fCost => gCost + hCost;
    private Node prevNode;
    public List<Node> neighbors;

    public GameObject NodeGO
    {
        get { return nodeGO; }
        set { nodeGO = value; }
    }
    public Vector2Int Position
    {
        get { return pos; }
    }
    public Node(GameObject nodeGO, Vector2Int pos)
    {
        this.nodeGO = nodeGO;
        this.pos = pos;
        prevNode = null;
    }
}
