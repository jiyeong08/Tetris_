using System;
using UnityEngine;

public class DualGame : MonoBehaviour
{
    
    [Header("GameSystem")]
    [SerializeField] private DualPlay dualPlay;
    
    [Header("GameUI")] 
    [SerializeField] private GameResult resultUI;
    [SerializeField] private UIManager playUI1;
    [SerializeField] private UIManager playUI2;
    
    [Header("GameBgm")]
    [SerializeField] private AudioSource playBgm;
    [SerializeField] private AudioSource endBgm;

    public event Action<Enum> DualGameFinish;
    
    public void Init()
    {
        playUI1.Init();
        playUI2.Init();
        resultUI.Init();
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        dualPlay.GameEndOn += gameEnd;

        resultUI.ToMain += goToMain;
        resultUI.Retry += retryGame;
    }
    
    private void OnDisable()
    {
        dualPlay.GameEndOn -= gameEnd;

        resultUI.ToMain -= goToMain;
        resultUI.Retry -= retryGame;
    }
    
    private void goToMain()
    {
        resultUI.GameEndOff();
        
        DualGameFinish?.Invoke(State.Main);
    }

    private void retryGame()
    {
        resultUI.GameEndOff();
        
        DualGameFinish?.Invoke(State.Dual);
    }
    
    private void gameEnd(string winner, int level1, int score1, int level2, int score2)
    {
        playBgm.Stop();
        playUI1.GamePlayUIOff();
        playUI2.GamePlayUIOff();
        resultUI.GameEndOn(winner, level1, score1, level2, score2);
        endBgm.Play();
    }
    
    public void GameStart(bool itemMode)
    {
        gameObject.SetActive(true);
        dualPlay.GameStart(itemMode);
        playBgm.Play();
        playUI1.GameStart();
        playUI2.GameStart();
    }

    public void Hide()
    {
        resultUI.GameEndOff();
        
        playUI1.GamePlayUIOff();
        playUI2.GamePlayUIOff();
        
        gameObject.SetActive(false);
    }
    
}
