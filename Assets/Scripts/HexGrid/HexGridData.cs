using System;
public enum HexType
{
    PointyTop = 0,
    FlatTop = 1,
}
[Serializable] public class HexGridData : GridBaseData
{
    public HexType hexType;
}
