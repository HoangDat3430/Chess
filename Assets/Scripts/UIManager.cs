using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    private UIManager() { }

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject promotionScreen;
    [SerializeField] Animator uiAnimatior;
    [SerializeField] GameObject chessboard;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    [SerializeField] private TMP_InputField ipAddress;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    public void OnLocalGameButtonClick()
    {
        uiAnimatior.SetTrigger("GameStart");
        Instantiate(chessboard);
        server.Init(8888);
        client.Init("10.3.1.103", 8888);
    }
    public void OnOnlineGameButtonClick()
    {
        uiAnimatior.SetTrigger("OnlineMenu");
    }
    public void OnConnectButtonClick()
    {
        client.Init(ipAddress.text, 8888);
    }
    public void OnHostButtonClick()
    {
        uiAnimatior.SetTrigger("HostMenu");
        server.Init(8888);
        client.Init("10.3.1.103", 8888);
    }
    public void OnOnlineBackButtonClick()
    {
        if(Chessboard.Instance != null)
        {
            Destroy(Chessboard.Instance.gameObject);
        }
        victoryScreen.SetActive(false);
        uiAnimatior.SetTrigger("GameMenu");
    }
    public void OnHostBackButtonClick()
    {
        uiAnimatior.SetTrigger("OnlineMenu");
        server.ShutDown();
        client.ShutDown();
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
    public void ShowResult(int teamLose)
    {
        int winner = teamLose == 0 ? 1 : 0;
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(teamLose).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(winner).gameObject.SetActive(true);
    }
    public void ResetGame()
    {
        victoryScreen.SetActive(false);
        Chessboard.Instance.ResetChessBoard();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
