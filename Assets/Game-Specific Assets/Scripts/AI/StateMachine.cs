using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class StateMachine
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
	
	public Action EvaluateState()
	{
		if(States == null
		   || States.Count == 0)
			throw new ArgumentException("States for a State Machine have not been initialized!");
		
		Action method = States[0].Behavior;
		int index = 0;
		
		foreach(var current in States)
		{
			Action newMethod = current.CheckState();
			if(newMethod != null)
			{
				method = newMethod;
				CurrentState = index;
			}
		}
		
		return method;
	}
	
	#endregion Methods
}

[Serializable]
public class State
{
	#region Variables / Properties
	
	public Func<bool> Condition;
	public Action Behavior;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public Action CheckState()
	{
		return Condition()
			? Behavior
			: null;
	}
	
	#endregion Methods
}


