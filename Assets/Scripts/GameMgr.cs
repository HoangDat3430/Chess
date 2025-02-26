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

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject promotionScreen;
    [SerializeField] Animator uiAnimatior;
    [SerializeField] GameObject chessboard;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    [SerializeField] private TMP_InputField ipAddress;
    [SerializeField] private GameObject[] cameraAngles;

    private int myTeam;
    private bool freezed = false;
    private bool isLocalGame = false;

    public static GameMgr Instance
    {
        get
        {
            return _instance;
        }
    }
    public int MyTeam
    {
        get
        {
            return myTeam;
        }
    }
    public bool Freezed
    {
        get
        {
            return freezed;
        }
    }
    public bool IsLocalGame
    {
        get
        {
            return isLocalGame;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    public void OnLocalGameButtonClick()
    {
        myTeam = 0;
        StartGame(true);
    }
    public void OnOnlineGameButtonClick()
    {
        uiAnimatior.SetTrigger("OnlineMenu");
    }
    public void OnConnectButtonClick()
    {
        client.Init(ipAddress.text, 7777);
    }
    public void OnHostButtonClick()
    {
        server.Init(7777);
        client.Init("127.0.0.1", 7777);
        uiAnimatior.SetTrigger("HostMenu");
    }
    public void OnOnlineBackButtonClick()
    {
        ShutDownRelay();
        victoryScreen.SetActive(false);
        uiAnimatior.SetTrigger("GameMenu");
        SwitchCameraAngle(CameraAngles.Menu);
    }
    public void OnHostBackButtonClick()
    {
        ShutDownRelay();
        uiAnimatior.SetTrigger("OnlineMenu");
    }
    public void ShowPromoteUI(int team)
    {
        freezed = true;
        promotionScreen.SetActive(true);
        promotionScreen.transform.Find("Waiting").gameObject.SetActive(team != myTeam);
        promotionScreen.transform.Find("Chosing").gameObject.SetActive(team == myTeam);
    }
    public void ConfirmPromote(int type)
    {
        PromoteReq(myTeam, type);
    }
    public void ResetGame()
    {
        RematchReq();
    }
    public void StartGame(bool isLocal)
    {
        uiAnimatior.SetTrigger("GameStart");
        isLocalGame = isLocal;
        if (Chessboard.Instance != null)
        {
            Destroy(Chessboard.Instance.gameObject);
        }
        Instantiate(chessboard);
        SwitchCameraAngle((CameraAngles)myTeam);
        freezed = false;
    }
    public void AssignTeam(int team)
    {
        this.myTeam = team;
    }
    public void MoveReq(int turn, ChessPiece cp, Vector2Int desPos, SpecialMove specialMove)
    {
        if(isLocalGame)
        {
            Vector2Int oriPos = new Vector2Int(cp.x, cp.y);
            Chessboard.Instance.MoveTo(oriPos, desPos, specialMove);
            Chessboard.Instance.UpdateTurn(turn + 1);
            return;
        }
        NetMove netMove = new NetMove();
        netMove.turn = turn;
        netMove.oriPosX = cp.x;
        netMove.oriPosY = cp.y;
        netMove.desPosX = desPos.x;
        netMove.desPosY = desPos.y;
        netMove.specialMove = (int)specialMove;
        client.SendToServer(netMove);
    }
    public void PromoteReq(int team, int type)
    {
        NetPromote netPromote = new NetPromote();
        netPromote.team = team;
        netPromote.type = type;
        client.SendToServer(netPromote);
    }
    public void ShowResultReq(int teamWin)
    {
        if (isLocalGame)
        {
            ShowResult(teamWin);
            return;
        }
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
        Chessboard.Instance.MoveTo(oriPos, desPos, (SpecialMove)netMove.specialMove);
        Chessboard.Instance.UpdateTurn(netMove.turn);
    }
    public void OnPromoteRes(NetworkMessage msg)
    {
        NetPromote netPromote = msg as NetPromote;
        freezed = false;
        Chessboard.Instance.Promote(netPromote.team, (ChessPieceType)netPromote.type);
        promotionScreen.SetActive(false);
    }
    public void OnShowResultRes(NetShowResult msg)
    {
        ShowResult(msg.teamWin);
    }
    public void OnRematchRes()
    {
        victoryScreen.SetActive(false);
        Chessboard.Instance.ResetChessBoard();
        freezed = false;
    }
    private void SwitchCameraAngle(CameraAngles angles)
    {
        for(int i = 0; i < cameraAngles.Length; i++)
        {
            cameraAngles[i].SetActive(false);
        }
        cameraAngles[(int)angles].SetActive(true);
    }
    private void ShutDownRelay()
    {
        if (Chessboard.Instance != null)
        {
            Destroy(Chessboard.Instance.gameObject);
        }
        server.ShutDown();
        client.ShutDown();
    }
    private void ShowResult(int teamWin)
    {
        victoryScreen.gameObject.SetActive(true);
        victoryScreen.transform.GetChild(0).gameObject.SetActive(teamWin == myTeam);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(teamWin != myTeam);
        freezed = true;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
