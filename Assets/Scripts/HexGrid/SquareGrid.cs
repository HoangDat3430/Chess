using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGrid : GridBase
{
    private SquareGridData _squareGridData;
    public override void Init(GridBaseData data)
    {
        _squareGridData = data as SquareGridData;
        base.Init(data);
    }
    public override void DrawGrid(GameObject newGrid, int x, int y)
    {
        Mesh mesh = new Mesh();
        newGrid.AddComponent<MeshFilter>().mesh = mesh;
        newGrid.AddComponent<MeshRenderer>().material = gridData.normalMat;

        mesh.vertices = SetVertices(4, x, y);
        mesh.triangles = new int[] { 0, 3, 2, 2, 1, 0 };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshCollider = newGrid.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }
    private Vector3[] SetVertices(int verticesCount, int x, int y)
    {
        float radius = gridData.edgeSize * Mathf.Sqrt(2) / 2;
        Vector3 center = new Vector3(x + radius, 0, y + radius);
        float angleOffset = 45; // place the first vertex of a square is on the top-right => cos(45) 
        Vector3[] vertices = new Vector3[verticesCount];
        for (int i = 0; i < verticesCount; i++)
        {
            var angleDeg = i * 90 + angleOffset; // each vertex is 90 degree apart
            var angleRad = Mathf.Deg2Rad * angleDeg;
            float posX = center.x + (radius - gridData.gridSpacing) * (float)Mathf.Cos(angleRad);
            float posY = center.z + (radius - gridData.gridSpacing) * (float)Mathf.Sin(angleRad);
            vertices[i] = new Vector3(posX, 0, posY);
        }
        return vertices;
    }
    private Vector2Int[] GetNeighborDir(Node node)
    {
        return new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            // 4 diagnose dirs in 8 dir mode 
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };
    }
    public override List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int dirCount = _squareGridData.isEightDir ? 8 : 4;
        Vector2Int[] directions = GetNeighborDir(node);
        for (int i = 0; i < dirCount; i++)
        {
            Vector2Int relatedPos = directions[i] + node.Position;
            if (relatedPos.x >= 0 && relatedPos.y >= 0 && relatedPos.x < gridData.mapWidth && relatedPos.y < gridData.mapHeight)
            {
                neighbors.Add(gridMap[relatedPos.x, relatedPos.y]);
            }
        }
        return neighbors;
    }
}
