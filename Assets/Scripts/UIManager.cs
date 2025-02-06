using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    private UIManager() { }

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject promotionScreen;
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
    private bool gameOver;
    private void Awake()
    {
        _instance = this;
        gameOver = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
