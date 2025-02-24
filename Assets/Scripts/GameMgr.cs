using TMPro;
using UnityEngine;

public enum CameraAngles
{
    White,
    Black,
    Menu,
}
public class GameMgr : MonoBehaviour
{
    private static GameMgr _instance;
    private GameMgr() { }

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject promotionScreen;
    [SerializeField] Animator uiAnimatior;
    [SerializeField] GameObject chessboard;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private GameObject[] cameraAngles;

    private int yourTeam;
    private bool matchEnd = false;

    public static GameMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameMgr();
            }
            return _instance;
        }
    }
    public int YourTeam
    {
        get
        {
            return yourTeam;
        }
    }
    public bool MatchIsEnd
    {
        get
        {
            return matchEnd;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    public void OnLocalGameButtonClick()
    {
        server.Init();
        client.Init(Server.ipAddress, Server.serverPort);
        StartGame();
    }
    public void OnOnlineGameButtonClick()
    {
        uiAnimatior.SetTrigger("OnlineMenu");
    }
    public void OnConnectButtonClick()
    {
        client.Init(ipAddress.text, Server.serverPort);
    }
    public void OnHostButtonClick()
    {
        server.Init();
        client.Init(Server.ipAddress, Server.serverPort);
        uiAnimatior.SetTrigger("HostMenu");
    }
    public void OnOnlineBackButtonClick()
    {
        if(Chessboard.Instance != null)
        {
            server.ShutDown();
            client.ShutDown();
            yourTeam = (int)CameraAngles.Menu;
            Destroy(Chessboard.Instance.gameObject);
        }
        victoryScreen.SetActive(false);
        uiAnimatior.SetTrigger("GameMenu");
    }
    public void OnHostBackButtonClick()
    {
        server.ShutDown();
        client.ShutDown();
        uiAnimatior.SetTrigger("OnlineMenu");
    }
    public void ShowPromoteUI()
    {
        promotionScreen.SetActive(true);
    }
    public void ConfirmPromote(int type)
    {
        Chessboard.Instance.Promote((ChessPieceType)type);
        promotionScreen.SetActive(false);
    }
    public void ResetGame()
    {
        RematchReq();
    }
    public void StartGame()
    {
        uiAnimatior.SetTrigger("GameStart");
        Instantiate(chessboard);
        SwitchCameraAngle();
        matchEnd = false;
    }
    public void AssignTeam(int team)
    {
        this.yourTeam = team;
    }
    public void MoveReq(int turn, ChessPiece cp, Vector2Int desPos)
    {
        NetMove netMove = new NetMove();
        netMove.turn = turn;
        netMove.oriPosX = cp.x;
        netMove.oriPosY = cp.y;
        netMove.desPosX = desPos.x;
        netMove.desPosY = desPos.y;
        client.SendToServer(netMove);
    }
    public void ShowResultReq(int teamWin)
    {
        NetShowResult netShowResult = new NetShowResult();
        netShowResult.teamWin = teamWin;
        client.SendToServer(netShowResult);
    }
    public void RematchReq()
    {
        client.SendToServer(new NetRematch());
    }
    public void OnMoveRes(NetworkMessage msg)
    {
        NetMove netMove = msg as NetMove;
        Vector2Int oriPos = new Vector2Int(netMove.oriPosX, netMove.oriPosY);
        Vector2Int desPos = new Vector2Int(netMove.desPosX, netMove.desPosY);
        Chessboard.Instance.MoveTo(oriPos, desPos);
        Chessboard.Instance.UpdateTurn(netMove.turn);
    }
    public void OnShowResultRes(NetShowResult msg)
    {
        Debug.LogError(msg.teamWin);
        victoryScreen.gameObject.SetActive(true);
        victoryScreen.transform.GetChild(0).gameObject.SetActive(msg.teamWin == yourTeam);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(msg.teamWin != yourTeam);
        matchEnd = true;
    }
    public void OnRematchRes()
    {
        victoryScreen.SetActive(false);
        Chessboard.Instance.ResetChessBoard();
        matchEnd = false;
    }
    private void SwitchCameraAngle()
    {
        for(int i = 0; i < cameraAngles.Length; i++)
        {
            cameraAngles[i].SetActive(false);
        }
        Debug.LogError(yourTeam);
        cameraAngles[yourTeam].SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
