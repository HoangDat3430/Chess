using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class King : ChessPiece
{
    public override void GetAvailableMoves()
    {
        base.GetAvailableMoves();
        Vector2Int[] directions = {
                    new (-1,  0 ), new ( 1,  0 ),  // Up, Down
                    new (0, -1), new (0, 1),  // Left, Right
                    new(-1, -1), new (-1, 1), // Top-Left, Top-Right
                    new(1, -1), new(1, 1)   // Bottom-Left, Bottom-Right
                };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int movePos = new Vector2Int(x + directions[i].x, y + directions[i].y);
            if (CanMove(movePos.x, movePos.y))
            {
                if(!Chessboard.Instance.GetRivalDangerZone().Contains(movePos))
                {
                    availableMoves.Add(movePos);
                    AddDangerZone(movePos);
                }
            }
        }
    }
    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movedList)
    {
        var kingMove = movedList.Find(m => m[0] == new Vector2Int(InitialPos.x, InitialPos.y));
        var leftRookMove = movedList.Find(m => m[0] == new Vector2Int(0, InitialPos.y));
        var rightRookMove = movedList.Find(m => m[0] == new Vector2Int(7, InitialPos.y)); ;

        if(kingMove == null)
        {
            if(leftRookMove == null || rightRookMove == null)
            {
                if (CasttlingCalculation(board))
                {
                    return SpecialMove.Castling;
                }
            }    
        }    
        return SpecialMove.None;
    }
    private bool CasttlingCalculation(ChessPiece[,] board)
    {
        bool casttlingAvailable = false;
        ChessPiece rookL = board[0, y];
        ChessPiece rookR = board[7, y];
        if (rookL != null && rookL.type == ChessPieceType.Rook && rookL.team == team)
        {
            if (board[x - 1, y] == null && board[x - 2, y] == null && board[x - 3, y] == null)
            {
                availableMoves.Add(new Vector2Int(x - 2, y));
                casttlingAvailable = true;
            }
        }
        if (rookR != null && rookR.type == ChessPieceType.Rook && rookR.team == team)
        {
            if (board[x + 1, y] == null && board[x + 2, y] == null)
            {
                availableMoves.Add(new Vector2Int(x + 2, y));
                casttlingAvailable = true;
            }
        }
        return casttlingAvailable;
    }    
}
