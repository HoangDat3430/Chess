using System.Collections.Generic;
using UnityEngine;


public class HexGrid : GridBase
{
    private HexGridData _hexGridData;
    public override void Init(GridBaseData data)
    {
        _hexGridData = data as HexGridData;
        base.Init(_hexGridData);
    }
    public override void DrawGrid(GameObject newGrid, int x, int y)
    {
        Mesh mesh = new Mesh();
        newGrid.AddComponent<MeshFilter>().mesh = mesh;
        newGrid.AddComponent<MeshRenderer>().material = gridData.normalMat;

        mesh.vertices = SetVertices(7, x, y);
        mesh.triangles = new int[] { 0, 1, 6, 0, 6, 5, 0, 5, 4, 0, 4, 3, 0, 3, 2, 0, 2, 1 };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshCollider = newGrid.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
    private Vector3[] SetVertices(int verticesCount, int x, int y)
    {
        Vector3 center = GetCenter(x, y);
        Vector3[] vertices = new Vector3[verticesCount];
        vertices[0] = center;
        //Place the first vertex of the flat top hexagon at a distance = cos(0) from the center. For a pointy top, it is 30 degrees from this point.
        int angleOffset = _hexGridData.hexType == HexType.FlatTop ? 0 : 30;
        for (int i = 1; i < verticesCount; i++)
        {
            float angleDeg = i * 60 + angleOffset; //In a regular hexagon, each vertex is 60 degrees apart.
            float angleRad = Mathf.Deg2Rad * angleDeg;
            float posX = center.x + (gridData.edgeSize - gridData.gridSpacing) * Mathf.Cos(angleRad);
            float posY = center.z + (gridData.edgeSize - gridData.gridSpacing) * Mathf.Sin(angleRad);
            vertices[i] = new Vector3(posX, 0, posY);
        }
        return vertices;
    }
    public override Vector3 GetCenter(int x, int y)
    {
        float centerX = _hexGridData.hexType == HexType.FlatTop ? x * (Height / 2 + (float)gridData.edgeSize / 2) : x * Width + (y % 2 == 0 ? 0 : Width / 2);
        float centerY = _hexGridData.hexType == HexType.FlatTop ? y * Width + (x % 2 == 0 ? 0 : Width / 2) : y * (Height / 2 + (float)gridData.edgeSize / 2);
        return new Vector3(centerX, 0, centerY);
    }
    private Vector2Int[] GetNeighborDir(Node node)
    {
        int diff = (_hexGridData.hexType == HexType.FlatTop ?node.Position.x : node.Position.y) % 2 == 0 ? -1 : 1;
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
    public override List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        
        foreach (var dir in GetNeighborDir(node))
        {
            Vector2Int relatedPos = (_hexGridData.hexType == HexType.PointyTop ? dir : new Vector2Int(dir.y, dir.x)) + node.Position; // rotate the direction
            if (relatedPos.x >= 0 && relatedPos.y >= 0 && relatedPos.x < gridData.mapWidth && relatedPos.y < gridData.mapHeight)
            {
                neighbors.Add(gridMap[relatedPos.x, relatedPos.y]);
            }
        }
        return neighbors;
    }
}
