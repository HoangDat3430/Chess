using UnityEngine;

public class Knight : ChessPiece
{
    protected override void Awake()
    {
        normalScale = 1.2f;
        base.Awake();
    }
    public override void GetAvailableMoves()
    {
        base.GetAvailableMoves();
        Vector2Int movePos = new Vector2Int(this.x + 1, this.y + 2);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x + 1, this.y - 2);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x - 1, this.y + 2);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x - 1, this.y - 2);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x + 2, this.y + 1);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x + 2, this.y - 1);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x - 2, this.y + 1);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
        movePos = new Vector2Int(this.x - 2, this.y - 1);
        if (CanMove(movePos.x, movePos.y))
        {
            CollideOpponent(movePos.x, movePos.y);
            availableMoves.Add(movePos);
            AddDangerZone(movePos);
        }
    }
}
