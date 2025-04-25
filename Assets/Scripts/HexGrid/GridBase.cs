using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class GridBase : IGrid
{
    public GridBaseData gridData;
    public float Width { get { return Mathf.Sqrt(3) * gridData.edgeSize; } }
    public float Height { get { return 2 * gridData.edgeSize; } }

    public Node[,] gridMap;
    public Node startPos;
    public Node goalPos;
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
    public virtual void Init(GridBaseData data)
    {
        gridData = data;
        GenGrid();
        SetNeighborsForAllGrid();
    }
    public virtual void GenGrid()
    {
        gridMap = new Node[gridData.mapWidth, gridData.mapHeight];
        for (int y = 0; y < gridData.mapHeight; y++)
        {
            for (int x = 0; x < gridData.mapWidth; x++)
            {
                gridMap[x, y] = GenSingleNode(x, y);
            }
        }
        SetStartPos(gridMap[0, 0]);
    }
    public virtual Node GenSingleNode(int x, int y)
    {
        GameObject newGrid = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        newGrid.transform.parent = GridMgr.Instance.transform;
        DrawGrid(newGrid, x, y);
        return new Node(newGrid, new Vector2Int(x, y));
    }
    public virtual void DrawGrid(GameObject newGrid, int x, int y)
    {
    }
    public virtual Vector3 GetCenter(int x, int y)
    {
        return Vector3.zero;
    }
    public virtual Node GetNodeByGameObject(GameObject go)
    {
        foreach (var node in gridMap)
        {
            if (node.NodeGO == go)
            {
                return node;
            }
        }
        return null;
    }
    public virtual void SetNeighborsForAllGrid()
    {
        for (int x = 0; x < gridData.mapWidth; x++)
        {
            for (int y = 0; y < gridData.mapHeight; y++)
            {
                gridMap[x, y].neighbors = GetNeighbors(gridMap[x, y]);
            }
        }
    }
    public virtual List<Node> GetNeighbors(Node node)
    {
        return null;
    }
}
