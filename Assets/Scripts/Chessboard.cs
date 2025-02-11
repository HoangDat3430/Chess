using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SpecialMove
{
    None,
    EnPassant,
    Castling,
    Promotion
}
public enum Layer
{
    Tile,
    Hover,
    Desired,
    Danger
}
public class Chessboard : MonoBehaviour
{
    private static Chessboard _instance;
    private Chessboard() {}
    public static Chessboard Instance
    {
        get 
        {
            if( _instance == null)
            {
                _instance = new Chessboard();
            }
            return _instance;
        }
    }
    [Header("Assets stuff")]
    [SerializeField] private Material[] tileMaterials;
    [SerializeField] private float tileSize = 0.1f;
    [SerializeField] private float yOffset = 0.01f;
    [SerializeField] private float deathSpace = 3f;
    [SerializeField] private float deathScale = 0.8f;

    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] chessBoard;
    private ChessPiece[,] chessPieces;
    private List<ChessPiece> allChessPieces = new List<ChessPiece>();
    private List<ChessPiece> deathBlack = new List<ChessPiece>();
    private List<ChessPiece> deathWhite = new List<ChessPiece>();
    private Camera curCamera;
    private Vector2Int curHover;
    private Vector3 bounds;
    private ChessPiece curSelected;
    private int turn = 1;
    private SpecialMove specialMove = SpecialMove.None;
    private List<Vector2Int[]> movesList = new List<Vector2Int[]>();
    private ChessPiece checkingChess = null;
    private List<Vector2Int> dangerZone = new List<Vector2Int>();

    public List<Vector2Int> DangerZone
    {
        get
        {
            return dangerZone;
        }
    }
    //for test
    private float curSizeTile;
    private float curYOffset;
    private void Awake()
    {
        _instance = this;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllChessPieces();
    }
    private void Update()
    {
        if (!curCamera)
        {
            curCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = curCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            Vector2Int hitPosition = MousePositionToBoardIndex(info.transform.gameObject);
            if (curHover != hitPosition)
            {
                if (curHover != -Vector2Int.one)
                {
                    SetTileLayer(curHover.x, curHover.y, Layer.Tile);
                }
                curHover = hitPosition;
                SetTileLayer(curHover.x, curHover.y, Layer.Hover);
            }
            if (Input.GetMouseButtonUp(0))
            {
                ClearHint();
                curSelected = chessPieces[curHover.x, curHover.y];
                if (curSelected != null)
                {
                    if(curSelected.team == turn % 2)
                    if(checkingChess == null || curSelected.type == ChessPieceType.King)
                    {
                        curSelected.GetAvailableMoves();
                        specialMove = curSelected.GetSpecialMove(ref chessPieces, ref movesList);
                        DisplayHint();
                    }    
                }
            }
        }
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Desired")))
        {
            if (curHover != -Vector2Int.one)
            {
                SetTileLayer(curHover.x, curHover.y, Layer.Tile);
                curHover = -Vector2Int.one;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector2Int hitPosition = MousePositionToBoardIndex(info.transform.gameObject);
                MoveTo(hitPosition);
                turn++;
            }
        }
        // for test
        if (curSizeTile != tileSize || curYOffset != yOffset)
        {
            curSizeTile = tileSize;
            curYOffset = yOffset;
            ClearAllTiles();
            GenerateAllTiles(curSizeTile, TILE_COUNT_X, TILE_COUNT_Y);
            //PositioningAllChessPieces();
        }
    }
    // Initialize the chessboard
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3(tileCountX/2 * tileSize, 0, tileCountY/2 * tileSize);
        chessBoard = new GameObject[tileCountX, tileCountY];
        for(int x = 0; x < tileCountX; x++){
            for(int y = 0; y < tileCountY; y++){
                chessBoard[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y){
        GameObject tile = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tile.transform.parent = transform;

        Mesh mesh = new Mesh();
        tile.AddComponent<MeshFilter>().mesh = mesh;
        tile.AddComponent<MeshRenderer>().material = tileMaterials[0];

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[2] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[]{ 0, 2, 3, 3, 1, 0 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        //mesh.RecalculateNormals();

        tile.AddComponent<BoxCollider>();

        tile.layer = LayerMask.NameToLayer("Tile");
        return tile;
    }
    private void SpawnAllChessPieces()
    {
        if (chessPieces == null)
        {
            chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        }
        // white team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, 1);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, 1);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 1] = SpawnSinglePiece(ChessPieceType.Pawn, 1);
        }
        // black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, 0);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, 0);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 6] = SpawnSinglePiece(ChessPieceType.Pawn, 0);
        }
        PositioningAllChessPieces();
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        GameObject chess = Instantiate(prefabs[(int)type - 1]);
        chess.transform.parent = transform;
        chess.GetComponentInChildren<MeshRenderer>().material = teamMaterials[team];
        ChessPiece chessPiece = chess.GetComponent<ChessPiece>();
        chessPiece.type = type;
        chessPiece.team = team;
        allChessPieces.Add(chessPiece);
        return chessPiece;
    }
    // Special moves
    private void ProcessingSpecialMove()
    {
        Vector2Int[] lastMove = movesList[movesList.Count - 1];
        switch (specialMove)
        {
            case SpecialMove.EnPassant:
                Vector2Int[] targetEnemy = movesList[movesList.Count - 2];
                if (lastMove[1].x == targetEnemy[1].x)
                {
                    EliminateChessPiece(chessPieces[targetEnemy[1].x, targetEnemy[1].y]);
                }
                break;
            case SpecialMove.Castling:
                if(lastMove[1].x - lastMove[0].x == 2) // casttling left
                {
                    chessPieces[lastMove[1].x - 1, lastMove[1].y] = chessPieces[7, lastMove[1].y];
                    chessPieces[7, lastMove[1].y] = null;
                    ChessPiecePositioning(lastMove[1].x - 1, lastMove[1].y);
                }
                else if (lastMove[1].x - lastMove[0].x == -2)
                {
                    chessPieces[lastMove[1].x + 1, lastMove[1].y] = chessPieces[0, lastMove[1].y];
                    chessPieces[0, lastMove[1].y] = null;
                    ChessPiecePositioning(lastMove[1].x + 1, lastMove[1].y);
                }
                break;
            case SpecialMove.Promotion:
                if (lastMove[1].y == 0 || lastMove[1].y == 7)
                {
                    UIManager.Instance.ShowPromoteUI();
                }
                break;
        }
        specialMove = SpecialMove.None;
    }
    public void Promote(ChessPieceType type)
    {
        turn++;
        Vector2Int[] promotionPos = movesList[movesList.Count - 1];
        ChessPiece pawn = chessPieces[promotionPos[1].x, promotionPos[1].y];
        pawn.x = -1; pawn.y = -1;
        pawn.gameObject.SetActive(false);
        chessPieces[promotionPos[1].x, promotionPos[1].y] = SpawnSinglePiece(type, turn % 2);
        chessPieces[promotionPos[1].x, promotionPos[1].y].isPromoted = true;
        allChessPieces.Add(chessPieces[promotionPos[1].x, promotionPos[1].y]);
        ChessPiecePositioning(promotionPos[1].x, promotionPos[1].y);
    }
    // Chesspiece positioning
    private void MoveTo(Vector2Int position)
    {
        ClearHint();
        ChessPiece cp = chessPieces[position.x, position.y];
        EliminateChessPiece(cp);
        movesList.Add(new Vector2Int[] { new Vector2Int(curSelected.x, curSelected.y), position });
        chessPieces[position.x, position.y] = chessPieces[curSelected.x, curSelected.y];
        chessPieces[curSelected.x, curSelected.y] = null;
        ChessPiecePositioning(position.x, position.y);
        ProcessingSpecialMove();
        CalculateDangerZone(turn % 2);
    }    
    private void EliminateChessPiece(ChessPiece cp)
    {
        if (cp == null) return;
        chessPieces[cp.x, cp.y] = null;
        cp.isDead = true;
        if (cp.team == 0)
        {
            cp.SetPosition(GetCenterTile(0, 7, cp.type == ChessPieceType.Pawn) - new Vector3(2 * tileSize, 2, deathWhite.Count * deathSpace - tileSize));
            deathWhite.Add(cp);
            cp.SetScale(deathScale);
        }
        else
        {
            cp.SetPosition(GetCenterTile(7, 0, cp.type == ChessPieceType.Pawn) + new Vector3(2 * tileSize, -2, deathBlack.Count * deathSpace - tileSize));
            deathBlack.Add(cp);
            cp.SetScale(deathScale);
        }
        if (cp.type == ChessPieceType.King)
        {
            UIManager.Instance.ShowResult(cp.team);
        }
    }
    private void PositioningAllChessPieces()
    {
        for(int x = 0; x < TILE_COUNT_X; x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    ChessPiecePositioning(x, y, true);
                }
            }
        }
    }
    private void ChessPiecePositioning(int x, int y, bool force = false)
    {
        chessPieces[x, y].x = x;
        chessPieces[x, y].y = y;
        chessPieces[x, y].SetPosition(GetCenterTile(x, y, chessPieces[x, y].type == ChessPieceType.Pawn), force);
        if (force && chessPieces[x, y].type == ChessPieceType.Knight)
        {
            Vector3 eulerAngles = chessPieces[x, y].transform.rotation.eulerAngles;
            eulerAngles.y = chessPieces[x, y].team == 0 ? 90 : -90;
            chessPieces[x, y].transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
    private Vector2Int MousePositionToBoardIndex(GameObject info)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessBoard[x, y] == info)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }
    public Vector3 GetCenterTile(int x, int y, bool isPawn)
    {   
        return new Vector3(x * tileSize, yOffset + (isPawn ? 1.2f : 0), y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    public void DisplayHint()
    {
        List<Vector2Int> moves = curSelected.AvailableMoves;
        for(int i = 0; i < moves.Count; i++)
        {   
            SetTileLayer(moves[i].x, moves[i].y, Layer.Desired);
        }
    }
    private void ClearHint()
    {
        if (!curSelected) return;
        List<Vector2Int> moves = curSelected.AvailableMoves;
        for (int i = 0; i < moves.Count; i++)
        {
            SetTileLayer(moves[i].x, moves[i].y, Layer.Tile);
        }
    }
    private void SetTileLayer(int x, int y, Layer layer, bool changeMask = true)
    {
        if(changeMask)
            chessBoard[x, y].layer = LayerMask.NameToLayer(layer.ToString());
        chessBoard[x, y].GetComponent<MeshRenderer>().material = tileMaterials[(int)layer];
    }
    // Checking legal moves
    private void CalculateDangerZone(int team)
    {
        dangerZone.Clear();
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                SetTileLayer(i, j, Layer.Tile);
            }
        }
        checkingChess = null;
        foreach (var cp in allChessPieces)
        {
            if (!IsValidPos(cp.x, cp.y) || cp.isDead) continue;
            if(cp.team == team)
            {
                cp.GetAvailableMoves();
                Debug.LogError(cp.AvailableMoves.Count);
            }
        }
        Debug.LogError(dangerZone.Count);
        foreach (var pos in dangerZone)
        {
            SetTileLayer(pos.x, pos.y, Layer.Danger, false);
        }
    }  
    public void AddToDangerZone(Vector2Int pos)
    {
        dangerZone.Add(pos);
    }    
    public bool CanMove(int x, int y, int team)
    {
        return IsValidPos(x, y) && (chessPieces[x, y] == null || chessPieces[x, y].team != team);
    }
    public bool CollideOpponent(ChessPiece checker, int x, int y, int team)
    {
        ChessPiece opponent = chessPieces[x, y];
        bool collided = opponent != null && opponent.team != team;
        if (opponent != null && opponent.type == ChessPieceType.King)
        {
            Debug.LogError("CHECKED!!!");
            checkingChess = checker;
        }
        return collided;
    }
    public bool IsValidPos(int x, int y)
    {
        return x >= 0 && x <= 7 && y >= 0 && y <= 7;
    }   
    // Game operations   
    public void ResetChessBoard()
    {
        ResetAllChessPieces();
        movesList.Clear();
        specialMove = SpecialMove.None;
    }
    public void ResetAllChessPieces()
    {
        foreach(ChessPiece cp in allChessPieces)
        {
            if (IsValidPos(cp.x, cp.y))
            {
                chessPieces[cp.x, cp.y] = null;
            }
            if (cp.isPromoted)
            {
                Destroy(cp.gameObject);
                continue;
            }
            cp.gameObject.SetActive(true);
            chessPieces[cp.InitialPos.x, cp.InitialPos.y] = cp;
            ChessPiecePositioning(cp.InitialPos.x, cp.InitialPos.y);
            cp.SetScale(cp.NormalScale);
        }
    }
    private void ClearAllTiles()
    {
        for(int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                Destroy(chessBoard[x, y]);
            }
        }
    }
}
