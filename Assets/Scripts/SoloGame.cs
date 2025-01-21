using System;
using UnityEngine;

public class SoloGame : MonoBehaviour
{
    
    [Header("GameSystem")]
    [SerializeField] private SoloPlay soloPlay;
    
    [Header("GameUI")] 
    [SerializeField] private GameResult resultUI;
    [SerializeField] private UIManager playUI;
    
    [Header("GameBgm")]
    [SerializeField] private AudioSource playBgm;
    [SerializeField] private AudioSource overBgm;
    [SerializeField] private AudioSource clearBgm;

    public event Action<Enum> SoloGameFinish;

    public void Init()
    {
        playUI.Init();
        resultUI.Init();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        soloPlay.GameOverOn += gameOver;
        soloPlay.GameClearOn += gameClear;
    
        resultUI.ToMain += goToMain;
        resultUI.Retry += retryGame;
    }
    
    private void OnDisable()
    {
        soloPlay.GameOverOn -= gameOver;
        soloPlay.GameClearOn -= gameClear;
        
        resultUI.ToMain -= goToMain;
        resultUI.Retry -= retryGame;
    }

    private void goToMain()
    {
        resultUI.GameClearOff();
        resultUI.GameOverOff();
        
        SoloGameFinish?.Invoke(State.Main);
    }

    private void retryGame()
    {
        resultUI.GameClearOff();
        resultUI.GameOverOff();
        
        SoloGameFinish?.Invoke(State.Solo);
    }

    private void gameOver(int score, int level)
    {
        playBgm.Stop();
        playUI.GamePlayUIOff();
        resultUI.GameOverOn(score, level);
        overBgm.Play();
    }

    private void gameClear(int score)
    {
        playBgm.Stop();
        playUI.GamePlayUIOff();
        resultUI.GameClearOn(score);
        clearBgm.Play();
    }

    public void GameStart(bool itemMode)
    {
        gameObject.SetActive(true);
        soloPlay.GameStart(itemMode);
        playBgm.Play();
        playUI.GameStart();
    }

    public void Hide()
    {
        resultUI.GameClearOff();
        resultUI.GameOverOff();
        
        playUI.GamePlayUIOff();
        
        gameObject.SetActive(false);
    }
    
}
