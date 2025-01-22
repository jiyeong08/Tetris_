using System;
using System.Collections.Generic;
using UnityEngine;

public class re_GameManager : MonoBehaviour
{
    [Header("GameSystem")]
    [SerializeField] private re_GamePlay player1;
    [SerializeField] private re_GamePlay player2;
    [SerializeField] private re_GamePlay player3;
    private int players;
    private List<int> deadPlayers;
    private int[] player1Record;
    private int[] player2Record;
    private int[] player3Record;
    
    [Header("GameUI")] 
    [SerializeField] private re_GameResult resultUI;
    [SerializeField] private GameObject playUI;
    
    [Header("GameBgm")]
    [SerializeField] private AudioSource playBgm;
    [SerializeField] private AudioSource overBgm;
    [SerializeField] private AudioSource clearBgm;
    [SerializeField] private AudioSource endBgm;

    public event Action<Enum> GameFinish;
    
    public void Init()
    {
        playUI.SetActive(false);
        
        resultUI.Init();
        gameObject.SetActive(false);
        
        deadPlayers = new List<int>();
    }
    
    private void OnEnable()
    {
        resultUI.ToMain += goToMain;
        resultUI.Retry += retryGame;

        if (player1 != null) player1.GameEndOn += playingUpdate;
        if (player2 != null) player2.GameEndOn += playingUpdate;
        if (player3 != null) player3.GameEndOn += playingUpdate;
    }
    
    private void OnDisable()
    {
        resultUI.ToMain -= goToMain;
        resultUI.Retry -= retryGame;
        
        if (player1 != null) player1.GameEndOn -= playingUpdate;
        if (player2 != null) player2.GameEndOn -= playingUpdate;
        if (player3 != null) player3.GameEndOn -= playingUpdate;
    }
        
    public void GameStart(int players, bool itemMode)
    {
        this.players = players;
        gameObject.SetActive(true);
        playUI.SetActive(true);
        playBgm.Play();
        
        player1?.GameStart(this.players, 1, itemMode, new re_GameKey.Player1Key());
        player2?.GameStart(this.players, 2, itemMode, new re_GameKey.Player2Key());
        player3?.GameStart(this.players, 3, itemMode, new re_GameKey.Player3Key());
    }
    
    private void playingUpdate(int player, string result)
    {
        int winner = 1;
        
        if (players == 1)
        {
            player1Record = new int[2];
            player1Record[0] = player1.levelVal;
            player1Record[1] = player1.scoreVal;
            gameResultOn(winner, player1Record, result);
        }
        else if (players >= 2)
        {
            if (result == "lose") deadPlayers.Add(player);
            
            if (deadPlayers?.Count == players - 1)
            {
                for (int i = 1; i <= players; i++) 
                { 
                    if (!deadPlayers.Contains(i)) winner = i;
                }
                if (player1)
                {
                    player1Record = new int[2];
                    player1Record[0] = player1.levelVal;
                    player1Record[1] = player1.scoreVal;
                }
                if (player2)
                {
                    player2Record = new int[2];
                    player2Record[0] = player2.levelVal;
                    player2Record[1] = player2.scoreVal;
                }
                if (player3)
                {
                    player3Record = new int[2];
                    player3Record[0] = player3.levelVal;
                    player3Record[1] = player3.scoreVal;
                }
                deadPlayers.Clear();
                if (players == 2) gameResultOn(winner, player1Record, player2Record);
                else if (players == 3) gameResultOn(winner, player1Record, player2Record, player3Record);
            }
        }
    }

    private void gameResultOn(int winner, int[] playerRecord, string result)
    {
        playBgm.Stop();
        playUI.SetActive(false);
        resultUI.GameResultOn(winner, playerRecord, result);
        if (result == "lose") overBgm.Play();
        else if (result == "win") clearBgm.Play();
    }
    
    private void gameResultOn(int winner, int[] playerRecord1, int[] playerRecord2)
    {
        playBgm.Stop();
        playUI.SetActive(false);
        resultUI.GameResultOn(winner, playerRecord1, playerRecord2);
        endBgm.Play();
    }
    
    private void gameResultOn(int winner, int[] playerRecord1, int[] playerRecord2, int[] playerRecord3)
    {
        playBgm.Stop();
        playUI.SetActive(false);
        resultUI.GameResultOn(winner, playerRecord1, playerRecord2, playerRecord3);
        endBgm.Play();
    }
    
    private void goToMain()
    {
        resultUI.GameResultOff();
        
        GameFinish?.Invoke(NewState.Main);
    }

    private void retryGame()
    {
        resultUI.GameResultOff();
        
        GameFinish?.Invoke(NewState.Game);
    }

    public void Hide()
    {
        resultUI.GameResultOff();
        playUI.SetActive(false);
        gameObject.SetActive(false);
    }
}
