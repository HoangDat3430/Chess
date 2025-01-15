using UnityEditor;
using UnityEngine;

public class King : ChessPiece
{
    public override void OnClicked()
    {
        base.OnClicked();
        int[,] directions = {
                    { -1,  0 }, { 1,  0 },  // Up, Down
                    { 0, -1 }, { 0,  1 },  // Left, Right
                    { -1, -1 }, { -1,  1 }, // Top-Left, Top-Right
                    { 1, -1 }, { 1,  1 }   // Bottom-Left, Bottom-Right
                };
        for (int i = 0; i < directions.Length / 2; i++)
        {
            Vector2Int movePos = new Vector2Int(x + directions[i, 0], y + directions[i, 1]);
            if (CanMove(movePos.x, movePos.y, team))
            {
                desiredMove.Add(movePos);
            }
        }
    }
}
