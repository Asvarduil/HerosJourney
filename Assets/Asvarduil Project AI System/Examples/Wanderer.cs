using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

/// <summary>
/// Programmer's Notes:
/// ----------------------------------------------------
/// Wanderer  -- Effort Lv.1
/// 
/// This script defines a basic entity that wanders
/// around aimlessly.
/// 
/// Conditions:
/// - Default
/// - When current action has expired
/// 
/// Behaviors:
/// - Execute the current action.
/// - Calculate new action, and expiration time.
/// </summary>
public class Wanderer : BaseAI
{
	#region Variables / Properties
	
	public bool hasAction = false;
	public float minDecideTime = 0.5f;
	public float maxDecideTime = 1.5f;
	public float moveSpeed = 1.0f;
	
	public List<Vector3> Directions;
	
	private Vector3 _direction;
	private float _nextDecision;

	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Use this for initialization
	public void Start () 
	{	
		// Ready the Finite State Machine.  We do this by passing the State Machine
		// construct a list of States, which are pairs of a Condition method name,
		// and a Behavior method name that gets fired if A) the condition holds true,
		// and B) the condition has priority.  A condition has priority if it's the
		// last condition to be true during an EvaluateState() call.
		_states = new StateMachine(new List<State> {
			new State(DefaultCondition, PrepareNewAction),
			new State(IsPerformingTask, KeepPerformingTask)
		});
	}
	
	// You don't have to do anything more in this example.
	// The Update function in the base AI class takes care of everything!
	
	#endregion Engine Hooks
	
	#region Conditions
	
	/// <summary>
	/// Determines if the AI is in the midst of performing a task.
	/// </summary>
	/// <returns>
	/// If doing something true, otherwise false.
	/// </returns>
	private bool IsPerformingTask()
	{
		return hasAction;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	/// <summary>
	/// Rolls an internal n-sided dice to figure out what to do next.
	/// </summary>
	private void PrepareNewAction()
	{
		if(Directions == null
		   || Directions.Count == 0)
			throw new ArgumentNullException("You must specify at least one Direction in the Directions list!");
		
		hasAction = true;
		int directionId = Random.Range(0, Directions.Count);
		_nextDecision = Time.time + Random.Range(minDecideTime, maxDecideTime);
		
		_direction = Directions[directionId];
		
		// Debug Text to use while developing or troubleshooting...
		if(debugMode)
		{
			Debug.Log("Generating new decision...");
			Debug.Log(String.Format("moving in direction #{0}, will recalculate at {1}", directionId, _nextDecision));
		}
	}
	
	/// <summary>
	/// Continues to perform the assigned task.
	/// 
	/// For the purposes of this demo, it's just moving in the assigned direction.
	/// </summary>
	private void KeepPerformingTask()
	{
		// Make our movement framerate-independent...
		Vector3 move = _direction * moveSpeed * Time.deltaTime;
		
		// Perform the actual movement.
		transform.position += move;
		
		// Update condition
		hasAction = Time.time < _nextDecision;
	}
	
	#endregion Behaviors
}
