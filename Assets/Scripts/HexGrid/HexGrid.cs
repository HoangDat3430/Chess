using System;
using UnityEngine;


public class HexGrid : MonoBehaviour
{
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
    private void Awake()
    {
        hexMap = new GameObject[mapWidth, mapHeight];
        GenHexMap();
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
        hexMap[0, 0].GetComponent<MeshRenderer>().material = startPosMat;
    }
    private GameObject GenSingleHecGrid(float x, float y)
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

        newHex.AddComponent<BoxCollider>();
        return newHex;
    }
    private void SetVertices(ref Vector3[] vertices, float x, float y)
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
    private Vector3 GetCenter(float x, float y)
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
