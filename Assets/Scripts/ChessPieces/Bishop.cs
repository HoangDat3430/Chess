public class Bishop : ChessPiece
{
    public override void GetAvailableMoves()
    {
        base.GetAvailableMoves();
        GetAllDiagnosePaths();
    }
}
