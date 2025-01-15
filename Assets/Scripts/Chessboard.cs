using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

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
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material desiredMaterial;
    [SerializeField] private float tileSize = 0.1f;
    [SerializeField] private float yOffset = 0.01f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSpace = 3f;
    [SerializeField] private float deathScale = 0.8f;

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
                    SetTransparentTile(curHover.x, curHover.y);
                }
                curHover = hitPosition;
                SetHoverTile(curHover.x, curHover.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (chessPieces[curHover.x, curHover.y] != null)
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
                SetTransparentTile(curHover.x, curHover.y);
                curHover = -Vector2Int.one;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector2Int hitPosition = MousePositionToBoardIndex(info.transform.gameObject);
                ChessPiece cp = chessPieces[hitPosition.x, hitPosition.y];
                if (cp != null)
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
                    if(cp.type == ChessPieceType.King)
                    {
                        GameManager.Instance.TheKingKilled(true);
                    }    
                }
                chessPieces[hitPosition.x, hitPosition.y] = chessPieces[curSelected.x, curSelected.y];
                chessPieces[curSelected.x, curSelected.y] = null;
                ChessPiecePositioning(hitPosition.x, hitPosition.y);
                ClearHint();
            }
        }
        if (curSizeTile != tileSize || curYOffset != yOffset)
        {
            curSizeTile = tileSize;
            curYOffset = yOffset;
            ClearAllTiles();
            GenerateAllTiles(curSizeTile, TILE_COUNT_X, TILE_COUNT_Y);
            //PositioningAllChessPieces();
        }
    }
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
        tile.AddComponent<MeshRenderer>().material = tileMaterial;

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
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, 0);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, 0);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, 0);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, 0);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, 0);
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 1] = SpawnSinglePiece(ChessPieceType.Pawn, 0);
        }
        // black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.King, 1);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.Queen, 1);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, 1);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, 1);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, 1);
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 6] = SpawnSinglePiece(ChessPieceType.Pawn, 1);
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
            eulerAngles.y = chessPieces[x, y].team == 1 ? 90 : -90;
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
    private Vector3 GetCenterTile(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    public void DisplayHint()
    {
        List<Vector2Int> moves = curSelected.DesiredMove;
        for(int i = 0; i < moves.Count; i++)
        {
            SetDesiredTile(moves[i].x, moves[i].y);
        }
    }
    private void ClearHint()
    {
        if (!curSelected) return;
        List<Vector2Int> moves = curSelected.DesiredMove;
        for (int i = 0; i < moves.Count; i++)
        {
            SetTransparentTile(moves[i].x, moves[i].y);
        }
    }
    private void SetTransparentTile(int x, int y)
    {
        chessBoard[x, y].layer = LayerMask.NameToLayer("Tile");
        chessBoard[x, y].GetComponent<MeshRenderer>().material = tileMaterial;
    }
    private void SetHoverTile(int x, int y)
    {
        chessBoard[x, y].layer = LayerMask.NameToLayer("Hover");
        chessBoard[x, y].GetComponent<MeshRenderer>().material = hoverMaterial;
    }
    private void SetDesiredTile(int x, int y)
    {
        chessBoard[x, y].layer = LayerMask.NameToLayer("Desired");
        chessBoard[x, y].GetComponent<MeshRenderer>().material = desiredMaterial;
    }
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
    public void ResetGame()
    {
        chessPieces = null;
        SpawnAllChessPieces();
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
