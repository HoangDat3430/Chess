using UnityEditor;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void GetAvailableMoves()
    {
        base.GetAvailableMoves();
        GetHorizontalPath(x, y);
        GetVerticalPath(y, x);
        GetAllDiagnosePaths();
    }
}
