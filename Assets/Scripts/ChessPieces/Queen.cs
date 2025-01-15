using UnityEditor;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void OnClicked()
    {
        base.OnClicked();
        GetHorizontalPath(x, y);
        GetVerticalPath(y, x);
        GetAllDiagnosePaths();
    }
}
