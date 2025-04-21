using System;
using UnityEngine;


public class HexGrid : MonoBehaviour
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

    private GameObject[,] hexMap;
    private GameObject startPos;
    private GameObject goalPos;

    public GameObject StartPos
    {
        get { return startPos; }
        set
        {
            if(startPos != value)
            {
                if(startPos != null)
                {
                    startPos.GetComponent<MeshRenderer>().material = normalHexMat;
                }
                startPos = value;
                startPos.GetComponent<MeshRenderer>().material = startPosMat;
                if(goalPos != null && goalPos != startPos)
                {
                    PathFinding.AStar(startPos, goalPos, hexMap);
                }
            }
        }
    }
    public GameObject GoalPos
    {
        get { return goalPos; }
        set
        {
            if (goalPos != value)
            {
                if (goalPos != null)
                {
                    goalPos.GetComponent<MeshRenderer>().material = normalHexMat;
                }
                goalPos = value;
                goalPos.GetComponent<MeshRenderer>().material = goalMat;
            }
            if(startPos != goalPos)
            {
                PathFinding.AStar(startPos, goalPos, hexMap);
            }
        }
    }
    private void Awake()
    {
        Instance = this;
        hexMap = new GameObject[mapWidth, mapHeight];
        GenHexMap();
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
                if(isSetGoalPos)
                {
                    GoalPos = hitInfo.collider.gameObject;
                }
                else
                {
                    StartPos = hitInfo.collider.gameObject;
                }
            }
        }
    }
    private void GenHexMap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                hexMap[x, y] = GenSingleHecGrid(x, y);
            }
        }
        StartPos = hexMap[0, 0];
    }
    private GameObject GenSingleHecGrid(int x, int y)
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
        return newHex;
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
            float posX = center.x + hexEdge * Mathf.Cos(angleRad);
            float posY = center.z + hexEdge * Mathf.Sin(angleRad);
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
        centerX += .5f;
        centerY += .5f;
        return new Vector3(centerX, 0, centerY);
    }
}
