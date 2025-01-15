using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameManager() { }
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
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
    public bool TheKingKilled(bool isDead)
    {
        Debug.LogError("Game is over");
        Chessboard.Instance.ResetGame();
        return gameOver = true;
    }    
}
