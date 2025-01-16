using UnityEditor;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override void OnClicked()
    {
        base.OnClicked();
        Vector2Int movePos = new Vector2Int(x, y + (team == 1 ? 1 : -1));
        if (CanMove(movePos.x, movePos.y, team) && !CollideOpponent(movePos.x, movePos.y, team))
        {
            desiredMove.Add(new Vector2Int(movePos.x, movePos.y));
            // First move of a pawn can through 2 tiles
            if(y == InitialPos.y)
            {
                Vector2Int fstMove = new Vector2Int(movePos.x, movePos.y + (team == 1 ? 1 : -1));
                if (CanMove(fstMove.x, fstMove.y, team) && !CollideOpponent(fstMove.x, fstMove.y, team))
                {
                    desiredMove.Add(new Vector2Int(fstMove.x, fstMove.y));
                }
            }
        }
        if (CanMove(movePos.x + 1, movePos.y, team) && CollideOpponent(movePos.x + 1, movePos.y, team))
        {
            desiredMove.Add(new Vector2Int(movePos.x + 1, movePos.y));
        }
        if (CanMove(movePos.x - 1, movePos.y, team) && CollideOpponent(movePos.x - 1, movePos.y, team))
        {
            desiredMove.Add(new Vector2Int(movePos.x - 1, movePos.y));
        }
    }
}
