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
        Vector2Int movePos = new Vector2Int(x, y + (team == 0 ? 1 : -1));
        if (CanMove(movePos.x, movePos.y) && !CollideOpponent(movePos.x, movePos.y))
        {
            availableMoves.Add(new Vector2Int(movePos.x, movePos.y));
            // First move of a pawn can through 2 tiles
            if (y == InitialPos.y)
            {
                Vector2Int fstMove = new Vector2Int(movePos.x, movePos.y + (team == 0 ? 1 : -1));
                if (CanMove(fstMove.x, fstMove.y) && !CollideOpponent(fstMove.x, fstMove.y))
                {
                    availableMoves.Add(new Vector2Int(fstMove.x, fstMove.y));
                }
            }
        }
        Vector2Int atkPos = new Vector2Int(movePos.x + 1, movePos.y);
        if (CanMove(atkPos.x, atkPos.y))
        {
            AddDangerZone(atkPos);
            if(CollideOpponent(atkPos.x, atkPos.y))
            {
                availableMoves.Add(atkPos);
            }
        }
        atkPos = new Vector2Int(movePos.x - 1, movePos.y);
        if (CanMove(atkPos.x, atkPos.y))
        {
            AddDangerZone(atkPos);
            if (CollideOpponent(atkPos.x, atkPos.y))
            {
                availableMoves.Add(atkPos);
            }
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
                        EnPassantPos = new Vector2Int(cp.x, y + (team == 0 ? 1 : -1));
                        availableMoves.Add(EnPassantPos);
                        return SpecialMove.EnPassant;
                    }
                }
            }
        }
        return SpecialMove.None;
    }
}
