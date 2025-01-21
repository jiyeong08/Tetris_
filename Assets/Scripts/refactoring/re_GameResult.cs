using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class re_GameResult : MonoBehaviour
{
    [Header("Game Result")]
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject gameClear;
    [SerializeField] private GameObject gameEnd;

    [Header("Buttons")] 
    [SerializeField] private Button overToMain;
    [SerializeField] private Button overToGame;
    [SerializeField] private Button clearToMain;
    [SerializeField] private Button clearToGame;
    [SerializeField] private Button endToMain;
    [SerializeField] private Button endToGame;
    
    public event Action ToMain;
    public event Action Retry;
    
    [Header("Game Record")]
    [SerializeField] private TMP_Text overRecord;
    [SerializeField] private TMP_Text clearRecord;
    [SerializeField] private TMP_Text player1Record;
    [SerializeField] private TMP_Text player2Record;
    [SerializeField] private TMP_Text player3Record;
    [SerializeField] private TMP_Text multiResult;

    public void Init()
    {
        overToMain.onClick.AddListener(() => { ToMain?.Invoke(); });
        overToGame.onClick.AddListener(() => { Retry?.Invoke(); });
        clearToMain.onClick.AddListener(() => { ToMain?.Invoke(); });
        clearToGame.onClick.AddListener(() => { Retry?.Invoke(); });
        endToMain.onClick.AddListener(() => { ToMain?.Invoke(); });
        endToGame.onClick.AddListener(() => { Retry?.Invoke(); });
        
        gameOver.SetActive(false);
        gameClear.SetActive(false);
        gameEnd.SetActive(false);
        
        gameObject.SetActive(false);
    }
    
    public void GameOverOn(int score, int level)
    {
        overRecord.text = "Score : " + score + "  Level : " + level;
        gameObject.SetActive(true);
        gameOver.SetActive(true);
    }
    
    public void GameClearOn(int score)
    {
        clearRecord.text = "Score : " + score;
        gameObject.SetActive(true);
        gameClear.SetActive(true);
    }
    
    public void GameResultOn(int winner, int[] record1, string result)
    {
        if (result == "lose")
        {
            overRecord.text = "Level : " + record1[0] + "  Score : " + record1[1];
            gameObject.SetActive(true);
            gameOver.SetActive(true);
        }
        else if (result == "win")
        {
            clearRecord.text = "Score : " + record1[1];
            gameObject.SetActive(true);
            gameClear.SetActive(true);
        }
    }
    
    public void GameResultOn(int winner, int[] record1, int[] record2)
    {
        multiResult.text = $"Player {winner} win!";
        player1Record.text = "Player 1"
                             + "\n\nLevel : " + record1[0]
                             + "\nScore : " + record1[1];
        player2Record.text = "Player 2"
                             + "\n\nLevel : " + record2[0]
                             + "\nScore : " + record2[1];
        player3Record.text = "Player 3"
                             + "\n\nNON PLAYER"
                             + "\n ";
        gameObject.SetActive(true);
        gameEnd.SetActive(true);
    }
    
    public void GameResultOn(int winner, int[] record1, int[] record2, int[] record3)
    {
        multiResult.text = $"Player {winner} win!";
        player1Record.text = "Player 1"
                             + "\n\nLevel : " + record1[0]
                             + "\nScore : " + record1[1];
        player2Record.text = "Player 2"
                             + "\n\nLevel : " + record2[0]
                             + "\nScore : " + record2[1];
        player3Record.text = "Player 3"
                             + "\n\nLevel : " + record3[0]
                             + "\nScore : " + record3[1];
        gameObject.SetActive(true);
        gameEnd.SetActive(true);
    }

    public void GameResultOff()
    {
        gameOver.SetActive(false);
        gameClear.SetActive(false);
        gameEnd.SetActive(false);
        gameObject.SetActive(false);
    }
}
