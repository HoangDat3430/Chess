using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private GameObject nodeGO;
    private Vector3 pos;
    public float mCost;
    public float hCost; // heuristic
    public float gCost = float.MaxValue; // cost so far
    public float fCost => gCost + hCost;
    public Node prevNode;
    public List<Node> neighbors;
    public bool isObstacle;
    public GameObject NodeGO
    {
        get { return nodeGO; }
        set { nodeGO = value; }
    }
    public Vector2 Position
    {
        get { return pos; }
    }
    public Node(GameObject nodeGO, Vector3 pos, bool isObstacle = false)
    {
        this.nodeGO = nodeGO;
        this.pos = pos;
        this.isObstacle = isObstacle;
        mCost = 1;
        prevNode = null;
    }
}
