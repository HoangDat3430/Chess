using System;
using System.Collections.Generic;
using UnityEngine;


public class HexGrid : MonoBehaviour, IGrid
{
    public static HexGrid Instance { get; private set; }
    public enum HexType
    {
        PointyTop = 0,
        FlatTop = 1,
    }
    [SerializeField] private int hexEdge = 1;
    [SerializeField] private int mapWidth = 2;
    [SerializeField] private int mapHeight = 4;
    [SerializeField] private Material normalHexMat;
    [SerializeField] private Material startPosMat;
    [SerializeField] private Material desiredMat;
    [SerializeField] private Material goalMat;
    [SerializeField] private HexType type;

    public float Width { get { return Mathf.Sqrt(3) * hexEdge; } }
    public float Height { get { return 2 * hexEdge; } }

    private Node[,] hexMap;
    private Node startPos;
    private Node goalPos;
    public Node StartPos
    {
        get { return startPos; }
        set
        {
            if (startPos != value)
            {
                if (startPos != null)
                {
                    startPos.NodeGO.GetComponent<MeshRenderer>().material = normalHexMat;
                }
                startPos = value;
                startPos.NodeGO.GetComponent<MeshRenderer>().material = startPosMat;
                if (goalPos != null && goalPos != startPos)
                {
                    foreach (var node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = normalHexMat;
                    }
                    PathFinding.AStar(startPos, goalPos);
                    foreach (Node node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = desiredMat;
                    }
                }
            }
        }
    }
    public Node GoalPos
    {
        get { return goalPos; }
        set
        {
            if (goalPos != value)
            {
                if (goalPos != null)
                {
                    goalPos.NodeGO.GetComponent<MeshRenderer>().material = normalHexMat;
                }
                goalPos = value;
                goalPos.NodeGO.GetComponent<MeshRenderer>().material = goalMat;
                if (startPos != null && startPos != goalPos)
                {
                    foreach (var node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = normalHexMat;
                    }
                    PathFinding.AStar(startPos, goalPos);
                    foreach (Node node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = desiredMat;
                    }
                }
            }
            
        }
    }
    private void Awake()
    {
        Instance = this;
        hexMap = new Node[mapWidth, mapHeight];
        GenGrid();
        SetNeighborsForAllGrid();
    }
    private void Update()
    {
        bool isSetStartPos = Input.GetMouseButtonUp(0);
        bool isSetGoalPos = Input.GetMouseButtonUp(1);
        if (isSetStartPos || isSetGoalPos)
        {
            RaycastHit hitInfo;
            Ray hit = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(hit, out hitInfo, 100))
            {
                Node hitNode = GetNodeByGameObject(hitInfo.collider.gameObject);
                if (isSetGoalPos && hitNode != null)
                {
                    GoalPos = hitNode;
                }
                else
                {
                    StartPos = hitNode;
                }
            }
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
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                hexMap[x, y] = GenSingleNode(x, y);
            }
        }
        StartPos = hexMap[0, 0];
    }
    public Node GenSingleNode(int x, int y)
    {
        GameObject newHex = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        newHex.transform.SetParent(transform);

        Mesh mesh = new Mesh();
        newHex.AddComponent<MeshFilter>().mesh = mesh;
        newHex.AddComponent<MeshRenderer>().material = normalHexMat;

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
        return new Node(newHex, vertices[0]);
    }
    private void SetVertices(ref Vector3[] vertices, int x, int y)
    {
        Vector3 center = GetCenter(x, y);
        vertices[0] = center;
        int angleOffset = type == HexType.FlatTop ? 0 : 30;
        for (int i = 1; i < 7; i++)
        {
            float angleDeg = i * 60 + angleOffset;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            float posX = center.x + (hexEdge-0.1f) * Mathf.Cos(angleRad);
            float posY = center.z + (hexEdge-0.1f) * Mathf.Sin(angleRad);
            vertices[i] = new Vector3(posX, 0, posY);
        }
    }
    public Vector3 GetCenter(int x, int y)
    {
        float centerX = type == HexType.FlatTop ? x * (Height + hexEdge) : x * Width;
        float centerY = type == HexType.FlatTop ? y * Width / 2 : y * (Height / 2 + (float)hexEdge / 2);
        if (y % 2 == 1)
        {
            centerX += type == HexType.FlatTop ? Height * (3f / 4) : (float)Width / 2;
        }
        return new Vector3(centerX, 0, centerY);
    }
    public Node[,] GetGridMap()
    {
        return hexMap;
    }
    private void SetNeighborsForAllGrid()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                hexMap[x,y].neighbors = GetNeighbors(new Vector2Int(x, y));
            }
        }
    }
    private List<Node> GetNeighbors(Vector2Int pos)
    {
        List<Node> neighbors = new List<Node>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
        };
        foreach (var dir in directions)
        {
            Vector2Int relatedPos = dir + pos;
            if (relatedPos.x > 0 && relatedPos.y > 0 && relatedPos.x < mapWidth && relatedPos.y < mapHeight)
            {
                neighbors.Add(hexMap[relatedPos.x, relatedPos.y]);
            }
        }
        return neighbors;
    }

}
