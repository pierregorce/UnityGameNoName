using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void Callback();

public class StateTransition<T> : System.IEquatable<StateTransition<T>>
{
    public T initState { get; private set; }
    public T endState { get; private set; }

    public StateTransition(T init, T end)
    {
        initState = init; endState = end;
    }

    public bool Equals(StateTransition<T> other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return initState.Equals(other.initState) && endState.Equals(other.endState);
    }

    public override int GetHashCode()
    {
        if ((initState == null || endState == null))
            return 0;

        unchecked
        {
            int hash = 17;
            hash = hash * 23 + initState.GetHashCode();
            hash = hash * 23 + endState.GetHashCode();
            return hash;
        }
    }
}

public class FiniteStateMachine<T>
{
    protected T mState;
    protected T mPrevState;
    protected bool mbLocked = false;
    public bool log = false;

    protected Dictionary<StateTransition<T>, System.Delegate> mTransitions;

    public FiniteStateMachine(bool log = false)
    {
        this.log = log;
        mTransitions = new Dictionary<StateTransition<T>, System.Delegate>();
    }

    public void Initialise(T state) { mState = state; }

    public void Advance(T nextState)
    {
        if (mbLocked)
            return;

        StateTransition<T> transition = new StateTransition<T>(mState, nextState);

        // Check if the transition is valid
        System.Delegate d;
        if (mTransitions.TryGetValue(transition, out d)) // new StateTransition(mState, nextState)
        {
            if (log)
            {
                Debug.Log("[FMS] Advancing to " + nextState + " state...");
            }

            if (d != null)
            {
                Callback c = d as Callback;
                c();
            }

            mPrevState = mState;
            mState = nextState;
        }
        else
        {
            if (log)
            {
                Debug.Log("[FMS] Cannot advance to " + nextState + " state");
            }
        }
    }

    public void AddTransition(T init, T end, Callback c)
    {
        StateTransition<T> tr = new StateTransition<T>(init, end);

        if (mTransitions.ContainsKey(tr))
        {
            if (log)
            {
                Debug.Log("[FSM] Transition: " + tr.initState + " - " + tr.endState + " exists already.");
            }
            return;
        }

        mTransitions.Add(tr, c);
        if (log)
        {
            Debug.Log("[FSM] Added transition " + mTransitions.Count + ": " + tr.initState + " - " + tr.endState + ", Callback: " + c);
        }

    }

    // Call this to prevent the state machine from leaving this state
    public void Lock() { mbLocked = true; }

    public void Unlock()
    {
        mbLocked = false;
        Advance(mPrevState);
    }

    public T GetState() { return mState; }
    public T GetPrevState() { return mPrevState; }
}