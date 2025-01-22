using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class FSMManager
{
    // Fields
    private static List<StateData> m_listStateDatas = new List<StateData>();
    private Enum m_currentState;

    // Pattern : Singleton
    private static FSMManager m_instance = null;
    public static FSMManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new FSMManager();
            }
            return m_instance;
        }
    }

    // Properties
    public Enum CurrentState { get => m_currentState; }

    // Methods : Logic
    public void Init()
    {
        while (m_listStateDatas.Count > 0)
        {
            m_listStateDatas[0].Clear();
            m_listStateDatas.RemoveAt(0);
        }
    }
    public void AddState(StateData stateData)
    {
        m_listStateDatas.Add(stateData);
    }
    public void Start(Enum state)
    {
        Debug.Log("State Start : " + state);
        m_currentState = state;

        StateDataBase stateDataBase = getStateData(m_currentState);
        if (stateDataBase.StateStart != null)
            stateDataBase.StateStart();
    }
    public void Finish(Object objComplete = null)
    {
        StateDataBase stateDataBase = getStateData(m_currentState);
        if (stateDataBase.StateFinish != null)
            stateDataBase.StateFinish(objComplete);

        if (stateDataBase.NextState != null)
            Start(stateDataBase.NextState);
        else if (stateDataBase.StateNext != null)
            Start(stateDataBase.StateNext(objComplete));
    }
    public void ForceChange(Enum state, Object objComplete = null)
    {
        StateDataBase stateDataBase = getStateData(m_currentState);
        if (stateDataBase.StateFinish != null)
            stateDataBase.StateFinish(objComplete);

        Start(state);
    }

    // Functions
    private StateDataBase getStateData(Enum state)
    {
        for (int i = 0; i < m_listStateDatas.Count; i++)
        {
            if (m_listStateDatas[i].State.ToString() == state.ToString())
                return m_listStateDatas[i];
        }
        throw new Exception("State (" + state + ") is not found!");
    }
}
