public class Rook : ChessPiece
{
    public override void GetAvailableMoves()
    {
        availableMoves.Clear();
        GetHorizontalPath(x, y);
        GetVerticalPath(y, x);
    }
}
