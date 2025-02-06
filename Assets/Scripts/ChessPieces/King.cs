using UnityEditor;
using UnityEngine;

public class King : ChessPiece
{
    public override void OnClicked()
    {
        base.OnClicked();
        Vector2Int[] directions = {
                    new (-1,  0 ), new ( 1,  0 ),  // Up, Down
                    new (0, -1), new (0, 1),  // Left, Right
                    new(-1, -1), new (-1, 1), // Top-Left, Top-Right
                    new(1, -1), new(1, 1)   // Bottom-Left, Bottom-Right
                };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int movePos = new Vector2Int(x + directions[i].x, y + directions[i].y);
            if (CanMove(movePos.x, movePos.y, team))
            {
                desiredMove.Add(movePos);
            }
        }
    }
}
