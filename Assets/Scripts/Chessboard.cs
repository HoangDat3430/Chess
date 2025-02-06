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
    Desired
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
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSpace = 3f;
    [SerializeField] private float deathScale = 0.8f;
    [SerializeField] private GameObject victoryScreen;

    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] chessBoard;
    private ChessPiece[,] chessPieces;
    private List<ChessPiece> deathBlack = new List<ChessPiece>();
    private List<ChessPiece> deathWhite = new List<ChessPiece>();
    private Camera curCamera;
    private Vector2Int curHover;
    private Vector3 bounds;
    private ChessPiece curSelected;
    private bool turn;
    private SpecialMove specialMove = SpecialMove.None;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();

    public List<Vector2Int[]> MoveList { get { return moveList; } }
    //for test
    private float curSizeTile;
    private float curYOffset;
    private void Awake()
    {
        _instance = this;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllChessPieces();
        turn = true;
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
                if (chessPieces[curHover.x, curHover.y] != null && chessPieces[curHover.x, curHover.y].team == Convert.ToInt32(turn))
                {
                    ClearHint();
                    curSelected = chessPieces[curHover.x, curHover.y];
                    curSelected.OnClicked();
                    DisplayHint();
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
                ClearHint();
                turn = !turn;
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
        bounds = new Vector3(tileCountX/2 * tileSize, 0, tileCountY/2 * tileSize) + boardCenter;
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
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.King, 0);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.Queen, 0);
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
        return chessPiece;
    }
    // Special moves
    private void ProcessingSpecialMove()
    {
        Debug.LogError(specialMove);
        switch (specialMove)
        {
            case SpecialMove.EnPassant:
                Vector2Int[] enPassantPos = moveList[moveList.Count - 2];
                EliminateChessPiece(chessPieces[enPassantPos[1].x, enPassantPos[1].y]);
                break;
            case SpecialMove.Castling:
                break;
            case SpecialMove.Promotion:
                Vector2Int[] promotionPos = moveList[moveList.Count - 1];
                chessPieces[promotionPos[1].x, promotionPos[1].y] = SpawnSinglePiece(ChessPieceType.Queen, turn ? 1 : 0);
                chessPieces[promotionPos[1].x, promotionPos[1].y].isTransformed = true;
                ChessPiecePositioning(promotionPos[1].x, promotionPos[1].y);
                turn = !turn;
                break;
        }
        specialMove = SpecialMove.None;
    }
    // Chesspiece positioning
    private void MoveTo(Vector2Int postion)
    {
        ChessPiece cp = chessPieces[postion.x, postion.y];
        if (cp != null)
        {
            EliminateChessPiece(cp);
        }
        moveList.Add(new Vector2Int[] { new Vector2Int(curSelected.x, curSelected.y), postion });
        chessPieces[postion.x, postion.y] = chessPieces[curSelected.x, curSelected.y];
        chessPieces[curSelected.x, curSelected.y] = null;
        ChessPiecePositioning(postion.x, postion.y);
        specialMove = chessPieces[postion.x, postion.y].GetSpecialMove();
        ProcessingSpecialMove();
    }    
    private void EliminateChessPiece(ChessPiece cp)
    {
        if (cp.team == 0)
        {
            cp.SetPosition(GetCenterTile(0, 7) - new Vector3(2 * tileSize, 2, deathWhite.Count * deathSpace - tileSize));
            deathWhite.Add(cp);
            cp.SetScale(deathScale);
        }
        else
        {
            cp.SetPosition(GetCenterTile(7, 0) + new Vector3(2 * tileSize, -2, deathBlack.Count * deathSpace - tileSize));
            deathBlack.Add(cp);
            cp.SetScale(deathScale);
        }
        if (cp.type == ChessPieceType.King)
        {
            ShowResult(cp.team);
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
        chessPieces[x, y].SetPosition(GetCenterTile(x, y), force);
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
    public Vector3 GetCenterTile(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    public ChessPiece GetChessPiece(int x, int y)
    {
        return chessPieces[x, y];
    }
    public void DisplayHint()
    {
        List<Vector2Int> moves = curSelected.DesiredMove;
        for(int i = 0; i < moves.Count; i++)
        {
            SetTileLayer(moves[i].x, moves[i].y, Layer.Desired);
        }
    }
    private void ClearHint()
    {
        if (!curSelected) return;
        List<Vector2Int> moves = curSelected.DesiredMove;
        for (int i = 0; i < moves.Count; i++)
        {
            SetTileLayer(moves[i].x, moves[i].y, Layer.Tile);
        }
    }
    private void SetTileLayer(int x, int y, Layer layer)
    {
        chessBoard[x, y].layer = LayerMask.NameToLayer(layer.ToString());
        chessBoard[x, y].GetComponent<MeshRenderer>().material = tileMaterials[(int)layer];
    }
    // Checking legal moves
    public bool CanMove(int x, int y, int team)
    {
        return IsValidPos(x, y) && (chessPieces[x, y] == null || chessPieces[x, y].team != team);
    }
    public bool CollideOpponent(int x, int y, int team)
    {
        return chessPieces[x, y] != null && chessPieces[x, y].team != team;
    }
    public bool IsValidPos(int x, int y)
    {
        return x >= 0 && x <= 7 && y >= 0 && y <= 7;
    }
    // Reset the game
    public void ResetChessBoard()
    {
        ResetChessPiecesOnBoard();
        ResetChessPiecesDeath(deathBlack);
        ResetChessPiecesDeath(deathWhite);
        moveList.Clear();
        specialMove = SpecialMove.None;
    }
    private void ResetChessPiecesOnBoard()
    {
        for(int x = 0; x < TILE_COUNT_X; x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    PostioningChessPieceToInitialPosition(chessPieces[x, y]);
                }
            }
        }
    }
    private void ResetChessPiecesDeath(List<ChessPiece> team)
    {
        for (int i = 0; i < team.Count; i++)
        {
            PostioningChessPieceToInitialPosition(team[i]);
        }
    }
    public void PostioningChessPieceToInitialPosition(ChessPiece cp)
    {
        if(chessPieces[cp.x, cp.y] != null)
        {
            chessPieces[cp.x, cp.y] = null;
        }
        chessPieces[cp.InitialPos.x, cp.InitialPos.y] = cp;
        ChessPiecePositioning(cp.InitialPos.x, cp.InitialPos.y);
        cp.SetScale(1.3f);
    }
    // UI
    private void ShowResult(int teamLose)
    {
        int winner = teamLose == 0 ? 1 : 0;
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(teamLose).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(winner).gameObject.SetActive(true);
    }
    public void ResetGame()
    {
        victoryScreen.SetActive(false);
        ResetChessBoard();
    }
    public void ExitGame()
    {
        Application.Quit();
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
