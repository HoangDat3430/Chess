using System;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    private Vector2Int EnPassantPos = new Vector2Int(-1, -1);

    protected override void Awake()
    {
        normalScale = 1.2f;
        base.Awake();
    }
    public override void GetAvailableMoves()
    {
        base.GetAvailableMoves();
        Vector2Int movePos = new Vector2Int(x, y + (team == 1 ? 1 : -1));
        if (CanMove(movePos.x, movePos.y, team) && !CollideOpponent(movePos.x, movePos.y, team))
        {
            availableMoves.Add(new Vector2Int(movePos.x, movePos.y));
            // First move of a pawn can through 2 tiles
            if (y == InitialPos.y)
            {
                Vector2Int fstMove = new Vector2Int(movePos.x, movePos.y + (team == 1 ? 1 : -1));
                if (CanMove(fstMove.x, fstMove.y, team) && !CollideOpponent(fstMove.x, fstMove.y, team))
                {
                    availableMoves.Add(new Vector2Int(fstMove.x, fstMove.y));
                }
            }
        }
        if (CanMove(movePos.x + 1, movePos.y, team) && CollideOpponent(movePos.x + 1, movePos.y, team))
        {
            availableMoves.Add(new Vector2Int(movePos.x + 1, movePos.y));
        }
        if (CanMove(movePos.x - 1, movePos.y, team) && CollideOpponent(movePos.x - 1, movePos.y, team))
        {
            availableMoves.Add(new Vector2Int(movePos.x - 1, movePos.y));
        }
    }
    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movedList)
    {
        foreach ( var m in availableMoves )
        {
            if(m.y == 0 || m.y == 7)
            {
                return SpecialMove.Promotion;
            }
        }
        if (movedList.Count > 0)
        {
            Vector2Int[] lastMove = movedList[movedList.Count - 1];
            ChessPiece cp = board[lastMove[1].x, lastMove[1].y];
            if (cp.type == ChessPieceType.Pawn && cp.team != team)
            {
                if (Math.Abs(lastMove[1].y - lastMove[0].y) == 2)
                {
                    if (lastMove[1].y == y)
                    {
                        EnPassantPos = new Vector2Int(cp.x, y + (team == 1 ? 1 : -1));
                        availableMoves.Add(EnPassantPos);
                        return SpecialMove.EnPassant;
                    }
                }
            }
        }
        return SpecialMove.None;
    }
}
