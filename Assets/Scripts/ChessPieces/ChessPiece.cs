using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6,
}
public class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public int x, y;
    public Vector2Int InitialPos = -Vector2Int.one;
    public int team;
    public bool isPromoted = false;
    public bool isDead = false;
    protected List<Vector2Int> availableMoves = new List<Vector2Int>();
    protected Vector3 desiredPosition = Vector3.one;
    protected Vector3 scale;
    protected float normalScale = 1.5f;
    protected Vector3 originalScale;
    protected SpecialMove specialMove = SpecialMove.None;
    public float NormalScale
    {
        get
        {
            return normalScale;
        }
    }
    public List<Vector2Int> AvailableMoves
    {
        get
        {
            return availableMoves;
        }
    }
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        SetScale(normalScale, true);
    }
    protected virtual void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * 10f);
    }
    public virtual void GetAvailableMoves()
    {
        //Debug.Log(string.Format("Selected {0} of team {1} at X:{2}, Y:{3}", type, team, x, y));
        availableMoves.Clear();
    }
    public virtual SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> movedList)
    {
        return SpecialMove.None;
    }
    public void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        } 
        if(!isPromoted && InitialPos == -Vector2Int.one)
        {
            SetInitPos(new Vector2Int(x, y));
        } 
    }
    public void SetScale(float scale, bool force = false)
    {
        this.scale = originalScale * scale;
        if (force)
        {
            transform.localScale = this.scale;
        }
    }
    protected void GetHorizontalPath(int dynamicX, int staticY)
    {
        for (int x = dynamicX + 1; x <= 7; x++)
        {
            if (CollideAlly(x, staticY)) break;
            if (!CanMove(x, staticY)) continue;
            Vector2Int pos = new Vector2Int(x, staticY);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, staticY)) break;
        }
        for (int x = dynamicX - 1; x >= 0; x--)
        {
            if (CollideAlly(x, staticY)) break;
            if (!CanMove(x, staticY)) continue;
            Vector2Int pos = new Vector2Int(x, staticY);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, staticY)) break;
        }
    }
    protected void GetVerticalPath(int dynamicY, int staticX)
    {
        for (int y = dynamicY + 1; y <= 7; y++)
        {
            if (CollideAlly(staticX, y)) break;
            if (!CanMove(staticX, y)) continue;
            Vector2Int pos = new Vector2Int(staticX, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(staticX, y)) break;
        }
        for (int y = dynamicY - 1; y >= 0; y--)
        {
            if (CollideAlly(staticX, y)) break;
            if (!CanMove(staticX, y)) continue;
            Vector2Int pos = new Vector2Int(staticX, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(staticX, y)) break;
        }
    }
    protected void GetAllDiagnosePaths()
    {
        for (int x = this.x + 1, y = this.y + 1; x <= 7 && y <= 7; x++, y++)
        {
            if (CollideAlly(x, y)) break;
            if (!CanMove(x, y)) continue;
            Vector2Int pos = new Vector2Int(x, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, y)) break;
        }
        for (int x = this.x - 1, y = this.y + 1; x >= 0 && y <= 7; x--, y++)
        {
            if (CollideAlly(x, y)) break;
            if (!CanMove(x, y)) continue;
            Vector2Int pos = new Vector2Int(x, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, y)) break;
        }
        for (int y = this.y - 1, x = this.x + 1; y >= 0 && x <= 7; y--, x++)
        {
            if (CollideAlly(x, y)) break;
            if (!CanMove(x, y)) continue;
            Vector2Int pos = new Vector2Int(x, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, y)) break;
        }
        for (int y = this.y - 1, x = this.x - 1; y >= 0 && x >= 0; y--, x--)
        {
            if (CollideAlly(x, y)) break;
            if (!CanMove(x, y)) continue;
            Vector2Int pos = new Vector2Int(x, y);
            availableMoves.Add(pos);
            AddDangerZone(pos);
            if (CollideOpponent(x, y)) break;
        }
    }
    public void SetInitPos(Vector2Int pos)
    {
        InitialPos = pos;
    }  
    protected bool CanMove(int x, int y)
    {
        return Chessboard.Instance.CanMove(this, x, y, team);
    }
    protected bool CollideOpponent(int x, int y)
    {
        return Chessboard.Instance.CollideOpponent(x, y, team);
    }
    protected bool CollideAlly(int x, int y)
    {
        return Chessboard.Instance.CollideAlly(x, y, team);
    }
    protected void AddDangerZone(Vector2Int pos)
    {
        Chessboard.Instance.AddToDangerZone(team, pos);
    }     
}
