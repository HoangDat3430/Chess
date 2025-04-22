using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrid
{
    public Node[,] GetGridMap();
    public void GenGrid();
    public Node GenSingleNode(int x, int y);
    public Vector3 GetCenter(int x, int y);
}
