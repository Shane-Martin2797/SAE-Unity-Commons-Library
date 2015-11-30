using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM<TContext>
    where TContext : class //TContext is the object this FSM will be attached to
{
    //Storing each state, and what type is maps to
    private Dictionary<System.Type, FSMState<TContext>> states = new Dictionary<System.Type, FSMState<TContext>>();

    //The current state of the FSM
    private FSMState<TContext> currentState;

    //If we should log all of the transition for this FSM
    public bool enableLogging = false;

    //If this FSM has been initialised
    private bool hasStarted = false;

    /// <summary>
    /// Get the context for the FSM
    /// </summary>
    public TContext context { get; private set; }

    /// <summary>
    /// If set to true, do not log errors about unregistered events
    /// </summary>
    public bool supressUnregisteredEventWarnings = false;

    /// <summary>
    /// Check if the FSM is in a given that
    /// </summary>
    /// <typeparam name="TCheckState">The type of the state to check</typeparam>
    /// <returns></returns>
    public bool isInState<TCheckState>()
        where TCheckState : FSMState<TContext>
    {
        if (currentState == null) return false;
        return currentState.GetType() == typeof(TCheckState);
    }

    private void Log(string message)
    {
        if (enableLogging)
        {
            Debug.Log(context.ToString() + ": " + message);
        }
    }


    /// <summary>
    /// Create a new FSM
    /// </summary>
    /// <param name="context">The context object of this FSM</param>
    public FSM(TContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Register a transition between one state and other for a given event id
    /// </summary>
    /// <typeparam name="TStartState">The start state</typeparam>
    /// <typeparam name="TDestinationState">The destination state</typeparam>
    /// <param name="eventId"></param>
    public void RegisterTransition<TStartState, TDestinationState>(string eventId)
        where TStartState : FSMState<TContext>
        where TDestinationState : FSMState<TContext>
    {
        FSMState<TContext> startState = states[typeof(TStartState)];

        if (startState == null)
        {
            Debug.LogError("State " + typeof(TStartState).Name + " has not been registered with the FSM");
            return;
        }

        startState.AddTransition<TDestinationState>(eventId);
    }

    /// <summary>
    /// Register a new state with the FSM.
    /// 
    /// If there is no default state set, the first state will be set to the default state/
    /// </summary>
    /// <typeparam name="TState">The type of the state to register</typeparam>
    /// <param name="isDefaultState">If this is true, this will be the default state for this FSM</param>
    public void RegisterState<TState>(bool isDefaultState = false)
        where TState : FSMState<TContext>, new()
    {
        //Create a new instance of this state
        TState state = new TState();
        state.SetFSM(this);

        //Chcek if the state exists, and if so, log a warning
        if (states.ContainsKey(state.GetType()))
        {
            Debug.LogWarning("State " + state.GetType().Name + " has already been registered with the FSM");
        }

        //Store this instance of the state mapped to a type
        states[state.GetType()] = state;

        //Set the default state if there is no other state, or we are overriding the default state
        if (isDefaultState || currentState == null)
        {
            currentState = state;
        }
    }

    void ProcessStart()
    {
        foreach (FSMState<TContext> state in states.Values)
        {
            state.RegisterTransitions();
        }

        Log("Entering default state " + currentState.GetType().Name);
        currentState.OnEnter();
        hasStarted = true;
    }

    public void FixedUpdate()
    {
        if (currentState == null) return;

        //If we haven't started yet, so do now
        if (!hasStarted)
        {
            ProcessStart();
        }

        //Notify the current state of an update
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    public void Update()
    {
        if (currentState == null) return;

        //If we haven't started yet, so do now
        if (!hasStarted)
        {
            ProcessStart();
        }

        //Notify the current state of an update
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    /// <summary>
    /// Transition to another state with a given event id
    /// </summary>
    /// <param name="eventId">The event id</param>
    public void Transition(string eventId)
    {
        //Find the next state type
        System.Type nextStateType = currentState.GetTransition(eventId);

        if (nextStateType != null)
        {
            //Update the state
            var previousState = currentState;

            if (!states.ContainsKey(nextStateType))
            {
                Debug.LogError("Attempt to transition to state " + nextStateType.Name + " but it has not been registered with the FSM");
                return;
            }

            currentState = states[nextStateType];

            //Send the exit to the old state
            Log("Leaving state " + currentState.GetType().Name);
            previousState.OnExit();

            Log("Entering state " + currentState.GetType().Name);
            //Notify the new state it has begun
            currentState.OnEnter();
        }
        else
        { //This event id isn't supported by the current state
            if (!supressUnregisteredEventWarnings)
            {
                Debug.LogWarning("State " + currentState.GetType().Name + " does not have a transition for event " + eventId);
            }
        }
    }

}