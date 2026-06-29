using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState: Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    
    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;
    void Awake()
    {
    }
    void Start()
    {
        CurrentState.EnterState();
    }
    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if ( !IsTransitioningState && nextStateKey.Equals( CurrentState.StateKey ) )
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToState( nextStateKey );
        }
            
    }
    private void OnTriggerEnter( Collider other )
    {
        CurrentState.OnTriggerEnter( other );
    }
    private void OnTriggerStay( Collider other )
    {
        CurrentState.OnTriggerStay( other );
    }
    private void OnTriggerExit( Collider other )
    {
        CurrentState.OnTriggerExit( other );
    }

    
    // Method to change states
    public void TransitionToState( EState key )
    {
        IsTransitioningState = true;
        // Exit the current state
        CurrentState.ExitState();

        // Change to the new state
        CurrentState = States[key];

        // Enter the new state
        CurrentState.EnterState();

        IsTransitioningState = false;

    }


}

// Interface for states
public abstract class BaseState<EState> where EState : Enum
{
    public BaseState( EState key )
    {
        StateKey = key;
    }

    public EState StateKey
    {
        get; private set;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter( Collider other );
    public abstract void OnTriggerExit( Collider other );
    public abstract void OnTriggerStay(Collider other );

}