using UnityEngine;

public class Knight : ChessPiece
{
    protected override void Awake()
    {
        originalScale = transform.localScale;
        SetScale(1.2f, true);
    }
    public override void OnClicked()
    {
        base.OnClicked();
        Vector2Int movePos = new Vector2Int(this.x + 1, this.y + 2);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x + 1, this.y - 2);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x - 1, this.y + 2);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x - 1, this.y - 2);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x + 2, this.y + 1);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x + 2, this.y - 1);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x - 2, this.y + 1);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
        movePos = new Vector2Int(this.x - 2, this.y - 1);
        if (CanMove(movePos.x, movePos.y, team))
        {
            desiredMove.Add(movePos);
        }
    }
}
