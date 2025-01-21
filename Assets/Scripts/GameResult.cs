using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
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
    [SerializeField] private TMP_Text dualResult;

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

    public void GameOverOff()
    {
        overRecord.text = null;
        gameOver.SetActive(false);
        gameObject.SetActive(false);
    }
    
    public void GameClearOff()
    {
        clearRecord.text = null;
        gameClear.SetActive(false);
        gameObject.SetActive(false);
    }

    public void GameEndOn(string winner, int level1, int score1, int level2, int score2)
    {
        dualResult.text = winner + " win!";
        player1Record.text = "Player 1"
                       + "\n\nLevel : " + level1
                       + "\nScore : " + score1;
        player2Record.text = "Player 2"
                       + "\n\nLevel : " + level2
                       + "\nScore : " + score2;
        gameObject.SetActive(true);
        gameEnd.SetActive(true);
    }

    public void GameEndOff()
    {
        player1Record.text = null;
        player2Record.text = null;
        gameEnd.SetActive(false);
        gameObject.SetActive(false);
    }

}
