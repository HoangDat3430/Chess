using System;
using UnityEngine;

public class Pawn : ChessPiece
{
    Vector2Int EnPassantPos = new Vector2Int(-1, -1);
    protected override void Awake()
    {
        originalScale = transform.localScale;
        SetScale(1.2f, true);
    }
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
        if (Chessboard.Instance.MoveList.Count > 0)
        {
            Vector2Int[] lastMove = Chessboard.Instance.MoveList[Chessboard.Instance.MoveList.Count - 1];
            ChessPiece cp = Chessboard.Instance.GetChessPiece(lastMove[1].x, lastMove[1].y);
            if (cp.type == ChessPieceType.Pawn && cp.team != team)
            {
                if (Math.Abs(lastMove[1].y - lastMove[0].y) == 2)
                {
                    if (lastMove[1].y == y)
                    {
                        EnPassantPos = new Vector2Int(cp.x, y + (team == 1 ? 1 : -1));
                        desiredMove.Add(EnPassantPos);
                        return;
                    }
                }
            }
        }
    }
    public override SpecialMove GetSpecialMove()
    {
        if(y == 0 || y == 7)
        {
            return SpecialMove.Promotion;
        }
        if(x == EnPassantPos.x && y == EnPassantPos.y)
        {
            return SpecialMove.EnPassant;
        }
        return SpecialMove.None;
    }
}
