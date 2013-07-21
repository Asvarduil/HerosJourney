using UnityEngine;
using System;
using System.Collections;

public class WaypointMapAI : MonoBehaviour 
{
	#region Enumerations
	
	/// <summary>
	/// Describes how the bot
	/// reacts to knowledge of
	/// the presence of a player.
	/// </summary>
	public enum Attitude
	{
		Ignore,
		Retreat,
		Attack
	}
	
	/// <summary>
	/// Describes how the bot
	/// chooses waypoints.
	/// </summary>
	public enum WaypointHabit
	{
		StandStill,
		FollowNext,
		RandomPoint
	}
	
	#endregion Enumerations
	
	#region Variables
	
	public string SensoryObject = "Sight";
	public Attitude ResponseToPlayer;
	public WaypointHabit WaypointDecision;
	
	/// <summary>
	/// The distance at which this entity chooses
	/// a new waypoint.
	/// </summary>
	public float WaypointSwitchDistance = 1.0f;
	
	/// <summary>
	/// The max difference that this entity can be rotated
	/// toward something before this object is considered
	/// to be facing that something.
	/// </summary>
	public float RotationMargin = 0.1f;
	
	/// <summary>
	/// Speed at which this entity turns, in degrees per second.
	/// </summary>
	public float TurnSpeed = 15;
	
	/// <summary>
	/// How quickly this entity moves, in World Units per second.
	/// </summary>
	public float Speed = 15;
	
	private SenseEntities _Sight;
	private Waypoint _CurrentWaypoint;
	
	#endregion Variables
	
	#region Engine Hooks
	
	// Use this for initialization
	void Start() 
	{
		_Sight = (SenseEntities) transform.Find(SensoryObject).GetComponent("SenseEntities");
		_CurrentWaypoint = Waypoint.FindClosest(transform.position);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(! OnHeroSpotted())
		{
			OnFollowWaypoint();
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	/// <summary>
	/// Follows waypoints, based upon the
	/// decision model.
	/// </summary>
	private void OnFollowWaypoint()
	{
		// If we're not close enough, perform the move,
		// but don't make a waypoint switch decision.
		if(Vector3.Distance (transform.position, _CurrentWaypoint.transform.position) > WaypointSwitchDistance)
		{
			MoveTowards(_CurrentWaypoint.transform.position);
			return;
		}
		
		switch(WaypointDecision)
		{		
			// TODO: Implement method to find next connected waypoint of least resistance.
			case WaypointHabit.FollowNext:
				_CurrentWaypoint = ChooseNextWaypoint();
				break;
			
			case WaypointHabit.RandomPoint:
				_CurrentWaypoint = _CurrentWaypoint.ChooseRandom();
				break;
			
			default:
				return;
		}
	}
	
	/// <summary>
	/// Reacts to players based upon attitude.
	/// </summary>
	private bool OnHeroSpotted()
	{
		if(! _Sight.SensedPlayer
		   || _Sight.LastSeenPlayer == null)
		{
			if(_Sight.LastSeenPlayer == null)
			{
				_Sight.SensedPlayer = false;
			}
			
			return false;
		}
		
		switch(ResponseToPlayer)
		{
			case Attitude.Ignore:
				return false;
				
			case Attitude.Retreat:
				MoveAwayFrom(_Sight.LastSeenPlayer.transform.position);
				break;
				
			case Attitude.Attack:
				MoveTowards(_Sight.LastSeenPlayer.transform.position);
				break;
				
			default:
				Debug.Log("Unexpected aggression state observed: " + ResponseToPlayer);
				return false;
		}
		
		return true;
	}
	
	/// <summary>
	/// Chooses the next waypoint to follow based
	/// on what waypoint is easiest to follow.
	/// </summary>
	/// <returns>
	/// The next waypoint.
	/// </returns>
	private Waypoint ChooseNextWaypoint()
	{
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		
		Waypoint best = _CurrentWaypoint;
		float bestDot = 0.000001f;
		
		foreach(Waypoint current in _CurrentWaypoint.Connected)
		{
			Vector3 direction = Vector3.Normalize(current.transform.position - transform.position);
			float dot = Vector3.Dot(direction, forward);
			
			//Debug.Log("For Waypoint " + current + ", the DOT is: " + dot);
			if ( dot >= bestDot && current != _CurrentWaypoint ) 
			{
				bestDot = dot;
				best = current;
			}			
		}
		
		return best;
	}
	
	/// <summary>
	/// Rotates towards a given 3D point.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	private void RotateTowards (Vector3 position)
	{
		Vector3 facing = position - transform.position;
		if(facing.magnitude < RotationMargin) { return; }
		
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(facing), TurnSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	
	/// <summary>
	/// Rotates away from a given 3D point.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	private void RotateAwayFrom (Vector3 position)
	{
		Vector3 facing = transform.position - position;	
		if(facing.magnitude < RotationMargin) { return; }
		
		// Rotate the game object.
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(facing), TurnSpeed * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	
	/// <summary>
	/// Moves towards a given 3D point.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	private void MoveTowards (Vector3 position)
	{
		RotateTowards(position);
		transform.Translate(Vector3.forward * Speed * Time.deltaTime);
	}
	
	/// <summary>
	/// Moves awya from a given 3D point.
	/// </summary>
	/// <param name='position'>
	/// Position.
	/// </param>
	private void MoveAwayFrom (Vector3 position)
	{
		RotateAwayFrom(position);
		transform.Translate(Vector3.forward * Speed * Time.deltaTime);
	}
	
	#endregion Methods
}
