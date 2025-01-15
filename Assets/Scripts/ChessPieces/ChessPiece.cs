using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
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
    public int team;
    private bool clicked = false;
    protected List<Vector2Int> desiredMove = new List<Vector2Int>();
    protected Vector3 desiredPosition = Vector3.one;
    protected Vector3 scale;
    protected Vector3 originalScale;

    public bool IsClicked { get { return clicked; } }
    public List<Vector2Int> DesiredMove
    {
        get
        {
            return desiredMove;
        }
    }
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        SetScale(1.3f, true);
    }
    protected virtual void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
        transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * 10f);
    }
    public virtual void OnClicked()
    {
        Debug.LogError(string.Format("Selected {0} of team {1} at X:{2}, Y:{3}", type, team, x, y));
        desiredMove.Clear();
    }
    public void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
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
            if (!CanMove(x, staticY, team)) break;
            desiredMove.Add(new Vector2Int(x, staticY));
            if (CollideOpponent(x, staticY, team)) break;
        }
        for (int x = dynamicX - 1; x >= 0; x--)
        {
            if (!CanMove(x, staticY, team)) break;
            desiredMove.Add(new Vector2Int(x, staticY));
            if (CollideOpponent(x, staticY, team)) break;
        }
    }
    protected void GetVerticalPath(int dynamicY, int staticX)
    {
        for (int y = dynamicY + 1; y <= 7; y++)
        {
            if (!CanMove(staticX, y, team)) break;
            desiredMove.Add(new Vector2Int(staticX, y));
            if (CollideOpponent(staticX, y, team)) break;
        }
        for (int y = dynamicY - 1; y >= 0; y--)
        {
            if (!CanMove(staticX, y, team)) break;
            desiredMove.Add(new Vector2Int(staticX, y));
            if (CollideOpponent(staticX, y, team)) break;
        }
    }
    protected void GetAllDiagnosePaths()
    {
        for (int x = this.x + 1, y = this.y + 1; x <= 7 && y <= 7; x++, y++)
        {
            if (!CanMove(x, y, team)) break;
            desiredMove.Add(new Vector2Int(x, y));
            if (CollideOpponent(x, y, team)) break;
        }
        for (int x = this.x - 1, y = this.y + 1; x >= 0 && y <= 7; x--, y++)
        {
            if (!CanMove(x, y, team)) break;
            desiredMove.Add(new Vector2Int(x, y));
            if (CollideOpponent(x, y, team)) break;
        }
        for (int y = this.y - 1, x = this.x + 1; y >= 0 && x <= 7; y--, x++)
        {
            if (!CanMove(x, y, team)) break;
            desiredMove.Add(new Vector2Int(x, y));
            if (CollideOpponent(x, y, team)) break;
        }
        for (int y = this.y - 1, x = this.x - 1; y >= 0 && x >= 0; y--, x--)
        {
            if (!CanMove(x, y, team)) break;
            desiredMove.Add(new Vector2Int(x, y));
            if (CollideOpponent(x, y, team)) break;
        }
    }
    protected bool CanMove(int x, int y, int team)
    {
        return Chessboard.Instance.CanMove(x, y, team);
    }
    protected bool CollideOpponent(int x, int y, int team)
    {
        return Chessboard.Instance.CollideOpponent(x, y, team);
    }
}
