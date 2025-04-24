using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private Material normalMat;
    [SerializeField] private Material startPosMat;
    [SerializeField] private Material desiredMat;
    [SerializeField] private Material goalPosMat;
    [SerializeField] private HexType hexType;

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
            if (startPos != value && value != goalPos)
            {
                if (startPos != null)
                {
                    startPos.NodeGO.GetComponent<MeshRenderer>().material = normalMat; //reset the previous start position
                }
                startPos = value;
                if (goalPos != null && goalPos != startPos)
                {
                    foreach (var node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = normalMat;
                    }
                    PathFinding.AStar(startPos, goalPos);
                    foreach (Node node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = desiredMat;
                    }
                }
                startPos.NodeGO.GetComponent<MeshRenderer>().material = startPosMat; // set color after previous result path cleared
            }
        }
    }
    public Node GoalPos
    {
        get { return goalPos; }
        set
        {
            if (goalPos != value && value != startPos)
            {
                if (goalPos != null)
                {
                    goalPos.NodeGO.GetComponent<MeshRenderer>().material = normalMat; // reset the previous goal position
                }
                goalPos = value;
                if (startPos != null)
                {
                    foreach (var node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = normalMat;
                    }
                    PathFinding.AStar(startPos, goalPos);
                    foreach (Node node in PathFinding.resultPath)
                    {
                        node.NodeGO.GetComponent<MeshRenderer>().material = desiredMat;
                    }
                }
                goalPos.NodeGO.GetComponent<MeshRenderer>().material = goalPosMat; // set color after previous result path cleared
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
        newHex.AddComponent<MeshRenderer>().material = normalMat;

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
        int angleOffset = hexType == HexType.FlatTop ? 0 : 30;
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
        float centerX = hexType == HexType.FlatTop ? x * (Height / 2 + (float)hexEdge / 2) : x * Width + (y % 2 == 0 ? 0 : Width / 2);
        float centerY = hexType == HexType.FlatTop ? y * Width + (x % 2 == 0 ? 0 : Width / 2) : y * (Height / 2 + (float)hexEdge / 2);
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
                hexMap[x,y].neighbors = GetNeighbors(hexMap[x, y]);
            }
        }
    }
    private Vector2Int[] GetNeighborDir(Node node)
    {
        int diff = (hexType == HexType.FlatTop ?node.Position.x : node.Position.y) % 2 == 0 ? -1 : 1;
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
            Vector2Int relatedPos = (hexType == HexType.PointyTop ? dir : new Vector2Int(dir.y, dir.x)) + node.Position; // rotate the direction
            if (relatedPos.x >= 0 && relatedPos.y >= 0 && relatedPos.x < mapWidth && relatedPos.y < mapHeight)
            {
                neighbors.Add(hexMap[relatedPos.x, relatedPos.y]);
            }
        }
        return neighbors;
    }

}
