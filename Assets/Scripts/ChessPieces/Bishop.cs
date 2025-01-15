using UnityEditor;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override void OnClicked()
    {
        base.OnClicked();
        GetAllDiagnosePaths();
    }
}
