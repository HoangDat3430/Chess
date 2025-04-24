using System;
public enum HexType
{
    PointyTop = 0,
    FlatTop = 1,
}
[Serializable] public class HexGridData : GridBaseData
{
    public int hexEdge = 1;

    public HexType hexType;
}
