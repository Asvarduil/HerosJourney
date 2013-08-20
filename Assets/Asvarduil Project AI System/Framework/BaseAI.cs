using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Programmer's Notes:
/// ---------------------------------------------
/// This code has the basic structures prepared for the AI system's boilerplate.
/// 
/// Each AI is tied to a FSM (Finite State Machine)
/// 	A FSM chooses the best action based on existing conditions.
/// 	In this API, a condition/behavior pair that is further down the list
/// 	has higher priority than a preceding one.
/// 
/// Every frame, we loop through all of the conditions to see what holds true.
/// 	For the highest-priority condition that holds true, we execute its
/// 	action.
/// 
/// The debugMode flag is included for ease of development, and writing your
/// 	own bug test code in an easy way.
/// 
/// The DefaultCondition and DefaultBehavior delegates are expected to be the
/// 	first entry in any AI script, but do not have to be.
/// </summary>
public class BaseAI : MonoBehaviour 
{
	#region Variables / Properties
	
	/// <summary>
	/// Debug Mode Flag, to ease debugging.
	/// </summary>
	public bool debugMode = false;
	
	/// <summary>
	/// The Finite State Machine.
	/// 
	/// FSMs should be initialized in Awake or Start, by creating a new
	/// StateMachine.  The constructor for StateMachine accepts a
	/// List<T> of States.  These states correspond to a pairing of a
	/// Condition (a function with no arguments that returns a bool),
	/// and an Action (a void function with no arguments.)
	/// </summary>
	protected IEvaluatesState _states;
	
	/// <summary>
	/// The action that will be executed.  This is declared
	/// as a class variable to reduce the number of allocations
	/// that the AI system causes.
	/// </summary>
	protected Action _behavior;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Basic action of an AI is to evaluate the operant state,
	// then to execute the accompanying action.
	public virtual void Update()
	{
		_states.EvaluateState();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	/// <summary>
	/// The Default Condition for an AI is always true.
	/// </summary>
	/// <returns>
	/// True, always.
	/// </returns>
	protected virtual bool DefaultCondition()
	{
		return true;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	/// <summary>
	/// The Default Action for an AI is to do nothing.
	/// </summary>
	protected virtual void DefaultAction()
	{
	}
	
	#endregion Behaviors
}
