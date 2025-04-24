using System;
using System.Collections.Generic;
using UnityEngine;


public class HexGrid : IGrid
{
    public HexGridData gridData;
    public float Width { get { return Mathf.Sqrt(3) * gridData.hexEdge; } }
    public float Height { get { return 2 * gridData.hexEdge; } }

    private Node[,] hexMap;
    private Node startPos;
    private Node goalPos;

    public HexGrid()
    {
        gridData = new HexGridData();
    }
    public void Init()
    {
        GenGrid();
        SetNeighborsForAllGrid();
    }
    public void SetStartPos(Node newStartNode)
    {
        if (startPos != newStartNode && newStartNode != goalPos)
        {
            if (startPos != null)
            {
                startPos.NodeGO.GetComponent<MeshRenderer>().material = gridData.normalMat; //reset the previous start position
            }
            startPos = newStartNode;
            if (goalPos != null && goalPos != startPos)
            {
                foreach (var node in PathFinding.resultPath)
                {
                    node.NodeGO.GetComponent<MeshRenderer>().material = gridData.normalMat;
                }
                PathFinding.AStar(startPos, goalPos);
                foreach (Node node in PathFinding.resultPath)
                {
                    node.NodeGO.GetComponent<MeshRenderer>().material = gridData.desiredMat;
                }
            }
            startPos.NodeGO.GetComponent<MeshRenderer>().material = gridData.startPosMat; // set color after previous result path cleared
        }
    }
    public void SetGoalPos(Node newGoalNode)
    {
        if (goalPos != newGoalNode && newGoalNode != startPos)
        {
            if (goalPos != null)
            {
                goalPos.NodeGO.GetComponent<MeshRenderer>().material = gridData.normalMat; // reset the previous goal position
            }
            goalPos = newGoalNode;
            if (startPos != null)
            {
                foreach (var node in PathFinding.resultPath)
                {
                    node.NodeGO.GetComponent<MeshRenderer>().material = gridData.normalMat;
                }
                PathFinding.AStar(startPos, goalPos);
                foreach (Node node in PathFinding.resultPath)
                {
                    node.NodeGO.GetComponent<MeshRenderer>().material = gridData.desiredMat;
                }
            }
            goalPos.NodeGO.GetComponent<MeshRenderer>().material = gridData.goalPosMat; // set color after previous result path cleared
        }
    }
    public Node GetNodeByGameObject(GameObject go)
    {
        foreach (var node in hexMap)
        {
            if (node.NodeGO == go)
            {
                return node;
            }
        }
        return null;
    }
    public void GenGrid()
    {
        hexMap = new Node[gridData.mapWidth, gridData.mapHeight];
        for (int y = 0; y < gridData.mapHeight; y++)
        {
            for (int x = 0; x < gridData.mapWidth; x++)
            {
                hexMap[x, y] = GenSingleNode(x, y);
            }
        }
        SetStartPos(hexMap[0, 0]);
    }
    public Node GenSingleNode(int x, int y)
    {
        GameObject newHex = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        newHex.transform.parent = GridMgr.Instance.transform;

        Mesh mesh = new Mesh();
        newHex.AddComponent<MeshFilter>().mesh = mesh;
        newHex.AddComponent<MeshRenderer>().material = gridData.normalMat;

        Vector3[] vertices = new Vector3[7];
        SetVertices(ref vertices, x, y);

        int[] tris = new int[] { 0, 1, 6, 0, 6, 5, 0, 5, 4, 0, 4, 3, 0, 3, 2, 0, 2, 1 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshCollider = newHex.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        return new Node(newHex, new Vector2Int(x, y));
    }
    private void SetVertices(ref Vector3[] vertices, int x, int y)
    {
        Vector3 center = GetCenter(x, y);
        vertices[0] = center;
        int angleOffset = gridData.hexType == HexType.FlatTop ? 0 : 30;
        for (int i = 1; i < 7; i++)
        {
            float angleDeg = i * 60 + angleOffset;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            float posX = center.x + (gridData.hexEdge-0.1f) * Mathf.Cos(angleRad);
            float posY = center.z + (gridData.hexEdge-0.1f) * Mathf.Sin(angleRad);
            vertices[i] = new Vector3(posX, 0, posY);
        }
    }
    public Vector3 GetCenter(int x, int y)
    {
        float centerX = gridData.hexType == HexType.FlatTop ? x * (Height / 2 + (float)gridData.hexEdge / 2) : x * Width + (y % 2 == 0 ? 0 : Width / 2);
        float centerY = gridData.hexType == HexType.FlatTop ? y * Width + (x % 2 == 0 ? 0 : Width / 2) : y * (Height / 2 + (float)gridData.hexEdge / 2);
        return new Vector3(centerX, 0, centerY);
    }
    public void SetNeighborsForAllGrid()
    {
        for (int x = 0; x < gridData.mapWidth; x++)
        {
            for (int y = 0; y < gridData.mapHeight; y++)
            {
                hexMap[x,y].neighbors = GetNeighbors(hexMap[x, y]);
            }
        }
    }
    private Vector2Int[] GetNeighborDir(Node node)
    {
        int diff = (gridData.hexType == HexType.FlatTop ?node.Position.x : node.Position.y) % 2 == 0 ? -1 : 1;
        return new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(diff, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(diff, -1),
        };
    }
    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        
        foreach (var dir in GetNeighborDir(node))
        {
            Vector2Int relatedPos = (gridData.hexType == HexType.PointyTop ? dir : new Vector2Int(dir.y, dir.x)) + node.Position; // rotate the direction
            if (relatedPos.x >= 0 && relatedPos.y >= 0 && relatedPos.x < gridData.mapWidth && relatedPos.y < gridData.mapHeight)
            {
                neighbors.Add(hexMap[relatedPos.x, relatedPos.y]);
            }
        }
        return neighbors;
    }

}
