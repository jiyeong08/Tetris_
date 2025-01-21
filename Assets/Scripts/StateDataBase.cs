using System;

public class StateDataBase
{
    // Delegate
    public delegate void OnStateStartDelegate();
    public delegate void OnStateFinishDelegate(System.Object objComplete);
    public delegate Enum OnStateNextDelegate(System.Object objComplete);

    // Fields
    private Enum m_state;
    private OnStateStartDelegate m_onStateStartDelegate;
    private OnStateFinishDelegate m_onStateFinishDelegate;
    private OnStateNextDelegate m_OnStateNextDelegate;
    private Enum m_stateNext;

    // Properties
    public Enum State { get => m_state; }
    public OnStateStartDelegate StateStart { get => m_onStateStartDelegate; }
    public OnStateFinishDelegate StateFinish { get => m_onStateFinishDelegate; }
    public OnStateNextDelegate StateNext { get => m_OnStateNextDelegate; }
    public Enum NextState { get => m_stateNext; }

    // Methods : Ctor.
    private StateDataBase() { }
    public StateDataBase(Enum state, OnStateStartDelegate onStateStartDelegate, OnStateFinishDelegate onStateFinishDelegate, OnStateNextDelegate onStateNextDelegate)
    {
        m_state = state;
        if (onStateStartDelegate != null)
            m_onStateStartDelegate = new OnStateStartDelegate(onStateStartDelegate);
        if (onStateFinishDelegate != null)
            m_onStateFinishDelegate = new OnStateFinishDelegate(onStateFinishDelegate);
        m_OnStateNextDelegate = new OnStateNextDelegate(onStateNextDelegate);
        m_stateNext = null;
    }
    public StateDataBase(Enum state, OnStateStartDelegate onStateStartDelegate, OnStateFinishDelegate onStateFinishDelegate, Enum stateNextDelegate)
    {
        m_state = state;
        if (onStateStartDelegate != null)
            m_onStateStartDelegate = new OnStateStartDelegate(onStateStartDelegate);
        if (onStateFinishDelegate != null)
            m_onStateFinishDelegate = new OnStateFinishDelegate(onStateFinishDelegate);
        m_OnStateNextDelegate = null;
        m_stateNext = stateNextDelegate;
    }

    // Methods
    public void Clear()
    {
        m_state = null;
        m_onStateStartDelegate = null;
        m_onStateFinishDelegate = null;
        m_OnStateNextDelegate = null;
    }
}
