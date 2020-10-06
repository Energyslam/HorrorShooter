using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour, IState
{
    private IState _currentState;
    public IState CurrentState { get { return _currentState; } }

    public void SetState(IState state)
    {
        if (state == _currentState)
        {
            return;
        }
        else
        {
            _currentState?.OnExit();
            _currentState = state;
            _currentState.OnEnter();
            //transition frame?
        }
    }

    public void Tick()
    {
        _currentState?.Tick();
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }
}
