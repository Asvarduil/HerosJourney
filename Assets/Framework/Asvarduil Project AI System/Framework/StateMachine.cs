using UnityEngine;
using System;
using System.Collections.Generic;

// Programmer's Notes:
// --------------------------------------------------------------
// A Finite State Machine is a construct with multiple states 
// that can act on only one at any given time.
//
// The construct determines its current operating state by checking
// for state transitions.  Take a turnstile, for instance; by default
// it is locked.  When a user deposits a ticket, the turnstile is
// unlocked.  When the user passes, the stil locks again, in
// preparation for the next user.
//
// Thus, you could say a turnstile is a Finite State Machine:
// [State 0]           Locked
// [Transition 0 -> 1] Ticket
// [State 1]           Unlocked
// [Transition 1 -> 0] User Passes
//
// We implement this with our State construct, which accepts a
// Behavior and a Condition.  The Condition represents the transition
// condition in the above; the Behavior causes the game object to 
// implement the required state in the game world.

/// <summary>
/// This class implements a Finite State Machine AI architecture,
/// to be expanded by classes that inherit from it.
/// </summary>
[Serializable]
public class StateMachine : IEvaluatesState
{
	#region Variables / Properties
	
	public List<State> States;
	public int CurrentState = 0;
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public StateMachine(List<State> states)
	{
		States = states;
	}
	
	#endregion Constructor
	
	#region Methods
	
	public void EvaluateState()
	{
		if(States == null
		   || States.Count == 0)
			throw new ArgumentException("States for a State Machine have not been initialized!");
		
		Action method = States[0].Behavior;
		for(int index = 0; index < States.Count; index++)
		{
			ICheckableState current = States[index];
			
			Action newMethod = current.CheckState();
			if(newMethod != null)
			{
				method = newMethod;
				CurrentState = index;
			}
		}
		
		if(method != null)
			method(); 
	}
	
	#endregion Methods
}


