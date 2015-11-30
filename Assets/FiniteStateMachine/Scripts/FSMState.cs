using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FSMState<TContext>
    where TContext : class
{
    //The FSM that owns this state
    protected FSM<TContext> fsm { get; private set; }

    //The context of the FSM
    protected TContext context { get { return fsm.context; } }

    /// <summary>
    /// A dictionary storing all of the transitions to other states and the events to trigger that transition
    /// </summary>
    private Dictionary<string, System.Type> map = new Dictionary<string, System.Type>();

    /// <summary>
    /// Add a new transition from this state to another
    /// </summary>
    /// <typeparam name="TDestinationState">The next state</typeparam>
    /// <param name="eventId">The event id to cause the transition</param>
    public void AddTransition<TDestinationState>(string eventId)
        where TDestinationState : FSMState<TContext>
    {
        map[eventId] = typeof(TDestinationState);
    }

    /// <summary>
    /// Get a state for a given event id
    /// </summary>
    /// <param name="eventId">The event id to check</param>
    /// <returns>If the transition is configured, the destination state, otherwise null</returns>
    public System.Type GetTransition(string eventId)
    {
        if (map.ContainsKey(eventId)) return map[eventId];
        else return null;
    }

    /// <summary>
    /// A co-routine helper for waiting then calling a callback. This is to be used with StartCoroutine
    /// </summary>
    /// <param name="time">The wait time</param>
    /// <param name="callback">The callback to call once the time has expired</param>
    /// <returns>An enumerator for StartCoroutine</returns>
    protected IEnumerator Wait(float time, System.Action callback = null)
    {
        yield return new WaitForSeconds(time);
        if (callback != null) callback();
    }

    public void SetFSM(FSM<TContext> owner)
    {
        fsm = owner;
    }

    /// <summary>
    /// Called once per state, provided a hook for configuring transitions for this state.
    /// </summary>
    public virtual void RegisterTransitions()
    {
    }

    /// <summary>
    /// Called on entry to this state.
    /// </summary>
    public virtual void OnEnter()
    {
    }


    /// <summary>
    /// Called on exit from this state.
    /// </summary>
    public virtual void OnExit()
    {
    }

    /// <summary>
    /// Called when this state is the current state in the fsm inside the main update loop.
    /// </summary>
    public virtual void Update()
    {
    }

    /// <summary>
    /// Called on fixed update - must call fsm.FixedUpdate() from the owner of this FSM
    /// </summary>
    public virtual void FixedUpdate()
    {
    }
}