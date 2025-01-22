using System;

public class StateData : StateDataBase
{
    // Methods : Ctor.
    public StateData(Enum state, OnStateStartDelegate onStateStartDelegate, OnStateFinishDelegate onStateFinishDelegate, OnStateNextDelegate onStateNexDelegate) : base(state, onStateStartDelegate, onStateFinishDelegate, onStateNexDelegate) { }
    public StateData(Enum state, OnStateStartDelegate onStateStartDelegate, OnStateFinishDelegate onStateFinishDelegate, Enum stateNext) : base(state, onStateStartDelegate, onStateFinishDelegate, stateNext) { }

}
