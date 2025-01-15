using UnityEditor;
using UnityEngine;

public class Rook : ChessPiece
{
    public override void OnClicked()
    {
        desiredMove.Clear();
        GetHorizontalPath(x, y);
        GetVerticalPath(y, x);
    }
}
