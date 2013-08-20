using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

/// <summary>
/// Programmer's Notes:
/// ----------------------------------------------------
/// Smart Wanderer  -- Effort Lv.2
/// 
/// This script defines a slightly smarter entity that 
/// wanders around aimlessly, until it sees something
/// that it should avoid!
/// 
/// This script introduces the concept of a sensor,
/// which is how an AI obtains input in order to make
/// more intelligent decisions.
/// 
/// Conditions:
/// - Default
/// - Has seen scary object
/// - When current action has expired
/// 
/// Behaviors:
/// - Execute the current action.
/// - Run away from scary object
/// - Calculate new action, and expiration time.
/// </summary>
public class SmartWanderer : BaseAI
{
	#region Variables / Properties
	
	public bool hasAction = false;
	public float minDecideTime = 0.5f;
	public float maxDecideTime = 1.5f;
	public float normalMoveSpeed = 1.0f;
	public float scaredMoveSpeed = 5.0f;
	
	public List<Vector3> Directions;
	
	private Vector3 _direction;
	private float _nextDecision;
	
	private ScaryObjectSense _sense;
	private bool _recentlyScared = false;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Use this for initialization
	void Start () 
	{
		// Preps reference to the scary object sense, which knows when a scary
		// object has been found, and what the scary object is.
		_sense = GetComponentInChildren<ScaryObjectSense>();
		
		// Ready the Finite State Machine.  We do this by passing the State Machine
		// construct a list of States, which are pairs of a Condition method name,
		// and a Behavior method name that gets fired if A) the condition holds true,
		// and B) the condition has priority.  A condition has priority if it's the
		// last condition to be true during an EvaluateState() call.
		_states = new StateMachine(new List<State> {
			new State(DefaultCondition, PrepareNewAction),
			new State(IsPerformingTask, KeepPerformingTask),
			new State(HasSeenScaryObject, FleeFromScaryObject)
		});
	}
	
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
	
	/// <summary>
	/// Determines whether this agent has seen a scary object.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance has seen a scary object; otherwise, <c>false</c>.
	/// </returns>
	private bool HasSeenScaryObject()
	{
		return _sense.IsScared;
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
		_recentlyScared = false;
		
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
	/// Flees from a detected scary object.
	/// </summary>
	private void FleeFromScaryObject()
	{
		// If an item hasn't been scared recently, determine
		// a means of escape!  This will be used until the
		// entity is no longer scared.
		if(! _recentlyScared)
		{
			// Treat this as having had an action given.
			hasAction = true;
			_recentlyScared = true;
			_nextDecision = Time.time + Random.Range(minDecideTime, maxDecideTime);
			
			// Just get away from the scary object, whatever the positioning!
			// After the direction has been determined, apply framerate independence and 
			// movement speed to this escape vector.
			_direction = transform.position - _sense.ScaryObjects[0].transform.position;
			_direction *= scaredMoveSpeed * Time.deltaTime;
		}
		
		PerformMove(_direction);
		
		if(debugMode)
			Debug.Log("Fleeing from scary object...");
	}
	
	/// <summary>
	/// Continues to perform the assigned task.
	/// 
	/// For the purposes of this demo, it's just moving in the assigned direction.
	/// </summary>
	private void KeepPerformingTask()
	{
		// Make our movement framerate-independent...
		Vector3 move = _direction * normalMoveSpeed * Time.deltaTime;
		PerformMove(move);
	}
	
	#endregion Behaviors
	
	#region Methods
	
	public void PerformMove(Vector3 move)
	{
		// Perform the actual movement.
		transform.position += move;
		
		// Update condition
		hasAction = Time.time < _nextDecision;
	}
	
	#endregion Methods
}
