using System;
using UnityEngine;

[Serializable] public abstract class GridBaseData
{
    public int edgeSize = 1;
    public float gridSpacing => edgeSize / 10;
    public int mapWidth = 2;
    public int mapHeight = 4;
    public Material normalMat;
    public Material startPosMat;
    public Material desiredMat;
    public Material goalPosMat;
}
