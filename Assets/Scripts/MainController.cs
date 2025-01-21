using System;
using UnityEngine;

public enum State
{
    Init, Main,
    Solo, 
    Dual, 
    Exit
}

public class MainController : MonoBehaviour
{
    [SerializeField] private MainPage mainPage;
    [SerializeField] private SoloGame soloGame;
    [SerializeField] private DualGame dualGame;

    private string mode;
    private bool itemMode;

    private FSMManager fsm = FSMManager.Instance;
    
    private void initSm()
    {
        fsm.Init();
        fsm.AddState(new StateData(State.Init, initStart, initFinish, State.Main));
        fsm.AddState(new StateData(State.Main, mainStart, mainFinish, mainNext));
        fsm.AddState(new StateData(State.Solo, soloStart, soloFinish, soloNext));
        fsm.AddState(new StateData(State.Dual, dualStart, dualFinish, dualNext));
        
        Debug.Log("States initialized");
    }

    private void Start()
    {
        init();
    }

    private void OnEnable()
    {
        mainPage.ButtonClick += onMainPageButtonClicked;

        dualGame.DualGameFinish += onDualGameFinished;

        soloGame.SoloGameFinish += onSoloGameFinished;
    }
    
    private void OnDisable()
    {
        mainPage.ButtonClick -= onMainPageButtonClicked;

        dualGame.DualGameFinish -= onDualGameFinished;

        soloGame.SoloGameFinish -= onSoloGameFinished;
    }

    private void init()
    {
        initSm();
        
        fsm.Start(State.Init);
    }

    private void initStart()
    {
        mainPage.Init();
        soloGame.Init();
        dualGame.Init();
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

    private Enum mainNext(object objcomplete)
    { 
        switch (mode)
        {
            case "Solo": 
                return State.Solo;
            case "Dual": 
                return State.Dual;
            case "Exit": 
                Application.Quit(); 
                break;
        }
        return null;
    }

    private void soloStart()
    {
        soloGame.GameStart(itemMode);
    }

    private void soloFinish(object objcomplete)
    {
        soloGame.Hide();
    }
    
    private Enum soloNext(object objcomplete)
    {
        switch (mode)
        {
            case "Solo":
                return State.Solo;
            case "Main":
                return State.Main;
        }
        return null;
    }

    private void dualStart()
    {
        dualGame.GameStart(itemMode);
    }

    private void dualFinish(object objcomplete)
    {
        dualGame.Hide();
    }

    private Enum dualNext(object objcomplete)
    {
        switch (mode)
        {
            case "Dual":
                return State.Dual;
            case "Main":
                return State.Main;
        }
        return null;
    }

    private void setMode(string mode)
    {
        this.mode = mode;
        fsm.Finish();
    }

    private void onMainPageButtonClicked(Enum mode, bool itemMode)
    {
        this.itemMode = itemMode;
        setMode(mode.ToString());
    }
    
    private void onDualGameFinished(Enum mode)
    {
        setMode(mode.ToString());
    }

    private void onSoloGameFinished(Enum mode)
    {
        setMode(mode.ToString());
    }

}