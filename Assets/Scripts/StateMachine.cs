using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineController<T> : MonoBehaviour where T : StateMachineController<T>
{
    public StateMachine<T> StateMachine
    {
        get;
        protected set;
    }

    private void Awake()
    {
        StateMachine = new StateMachine<T>((T)this);
    }

    private void Update()
    {
        StateMachine.Update();
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    public bool IsInState<StateType>()
    {
        return StateMachine.IsInState<StateType>();
    }

    public void SetState(IState<T> newState, bool force = false)
    {
        StateMachine.SetState(newState, force);
    }
}

public interface IState<T> where T : StateMachineController<T>
{
    void OnEnter(T controller);
    void OnExit(T controller);
    void OnUpdate(T controller);
}

public class StateMachine<T> where T : StateMachineController<T>
{
    private IState<T> _currentState;
    private T _controller;

    public IState<T> NextState
    {
        get;
        private set;
    }

    public StateMachine(T controller)
    {
        _controller = controller;
    }

    /// <summary>
    /// Changes current state machine state
    /// </summary>
    /// <param name="newState">New state to be set</param>
    /// <param name="force">Whether force state change when state of the same type is already set as current</param>
    public void SetState(IState<T> newState, bool force = false)
    {
        if(newState == null)
        {
            if(_currentState != null)
            {
                _currentState.OnExit(_controller);
            }
            _currentState = null;
            return;
        }

        if(_currentState == null)
        {
            _currentState = newState;
            _currentState.OnEnter(_controller);
        }
        else
        {
            if (_currentState.GetType() == newState.GetType() && !force)
            {
                return;
            }

            NextState = newState;
            _currentState.OnExit(_controller);
            _currentState = newState;
            _currentState.OnEnter(_controller);
        }
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.OnUpdate(_controller);
        }
    }

    public bool IsInState<StateType>()
    {
        return _currentState != null && _currentState.GetType() == typeof(StateType);
    }

    public IState<T> GetCurrentState()
    {
        return _currentState;
    }
}
