using System;
using UnityEngine;

public enum NewState
{
    Init, 
    Main,
    Game
    
}

public class re_MainController : MonoBehaviour
{
    [SerializeField] private re_MainPage mainPage;
    [SerializeField] private re_GameManager soloGameManager;
    [SerializeField] private re_GameManager dualGameManager;
    [SerializeField] private re_GameManager tripleGameManager;

    private int players;
    private string mode;
    private bool itemMode;
    private Enum resultBtn;

    private FSMManager fsm = FSMManager.Instance;
    
    private void initSm()
    {
        fsm.Init();
        fsm.AddState(new StateData(NewState.Init, initStart, initFinish, NewState.Main));
        fsm.AddState(new StateData(NewState.Main, mainStart, mainFinish, NewState.Game));
        fsm.AddState(new StateData(NewState.Game, gameStart, gameFinish, gameNext));
        
        Debug.Log("States initialized");
    }

    private void Start()
    {
        init();
    }

    private void OnEnable()
    {
        mainPage.ButtonClick += onMainPageButtonClicked;

        soloGameManager.GameFinish += onGameFinished;
        dualGameManager.GameFinish += onGameFinished;
        tripleGameManager.GameFinish += onGameFinished;

    }
    
    private void OnDisable()
    {
        mainPage.ButtonClick -= onMainPageButtonClicked;
        
        soloGameManager.GameFinish -= onGameFinished;
        dualGameManager.GameFinish -= onGameFinished;
        tripleGameManager.GameFinish -= onGameFinished;

    }

    private void init()
    {
        initSm();
        
        fsm.Start(NewState.Init);
    }

    private void initStart()
    {
        mainPage.Init();
        soloGameManager.Init();
        dualGameManager.Init();
        tripleGameManager.Init();
        fsm.Finish();
    }

    private void initFinish(object objcomplete) { }

    private void mainStart()
    {
        mainPage.Show();
    }

    private void mainFinish(object objcomplete)
    {
        mainPage.Hide();
    }

    private void gameStart()
    {
        switch (players)
        {
            case 1:
                soloGameManager.GameStart(players, itemMode);
                break;
            case 2:
                dualGameManager.GameStart(players, itemMode);
                break;
            case 3:
                tripleGameManager.GameStart(players, itemMode);
                break;
        }
    }

    private void gameFinish(object objcomplete)
    {
        switch (players)
        {
            case 1:
                soloGameManager.Hide();
                break;
            case 2:
                dualGameManager.Hide();
                break;
            case 3:
                tripleGameManager.Hide();
                break;
        }
    }

    private Enum gameNext(object objcomplete)
    {
        return resultBtn;
    }

    private void onMainPageButtonClicked(int players, bool itemMode)
    {
        this.players = players;
        this.itemMode = itemMode;
        fsm.Finish();
    }

    private void onGameFinished(Enum clickedBtn)
    {
        resultBtn = clickedBtn;
        fsm.Finish();
    }
}
