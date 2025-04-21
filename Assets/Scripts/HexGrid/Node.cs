using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private GameObject nodeGO;
    private float gCost;
    private float hCost;
    private float fCost => gCost + hCost;
    private Node prevNode;
    private List<Node> neighbors;

    public GameObject NodeGO
    {
        get { return nodeGO; }
        set { nodeGO = value; }
    }
    public Node(GameObject nodeGO)
    {
        this.nodeGO = nodeGO;
        prevNode = null;
    }
}
