using UnityEngine;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour 
{
	#region Constants
	
	/// <summary>
	/// Best likability value a waypoint can have.
	/// Used in ChooseRandom().  (Current Value: 10)
	/// </summary>
	private const int _Favorite = 10;
	
	#endregion Constants
	
	#region Variables
	
	public Texture2D waypointGizmo;
	
	public static Waypoint[] Waypoints;
	public List<Waypoint> Connected;
	
	#endregion Variables
	
	#region Engine Hooks
	
	public void Start()
	{
		RebuildWaypointList();
	}
	
	public void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, 0.25f);
		//Gizmos.DrawIcon(transform.position, waypointGizmo.name);
	}
	
	public void OnDrawGizmosSelected()
	{
		if(Waypoints == null
		   || Waypoints.Length == 0) 
		{ 
			RebuildWaypointList();
		}
		
		foreach(Waypoint current in Connected)
		{
			if(Physics.Linecast (transform.position, current.transform.position))
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.green;
			}
			
			Gizmos.DrawLine(transform.position, current.transform.position);
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	/// <summary>
	/// Finds the waypoint nearest the given location.
	/// </summary>
	/// <returns>
	/// The closest waypoint.
	/// </returns>
	/// <param name='position'>
	/// Position to evaluate from.
	/// </param>
	public static Waypoint FindClosest(Vector3 position)
	{
		Waypoint closest = null;
		float searchRange = 10000.0f;
		
		if(Waypoints == null
		   || Waypoints.Length == 0)
		{
			var waypoints = (Waypoint[]) GameObject.FindObjectsOfType (typeof(Waypoint));
			foreach(Waypoint current in waypoints)
			{
				current.RebuildWaypointList();
			}
		}
		
		foreach(Waypoint current in Waypoints)
		{
			float distance = Vector3.Distance(position, current.transform.position);
			
			if(distance < searchRange)
			{
				// Trim down the search range, set
				// the current point as the closest.
				searchRange = distance;
				closest = current;
			}
		}
		
		return closest;
	}
	
	[ContextMenu("Refresh Waypoint Connections")]
	public void RefreshConnections()
	{
		RebuildWaypointList();
	}
	
	/// <summary>
	/// Chooses a random connected waypoint.
	/// </summary>
	/// <returns>
	/// A random connected waypoint.
	/// </returns>
	public Waypoint ChooseRandom()
	{
		Waypoint destination = this;
		int favorite = 0;
		
		foreach(Waypoint current in Connected)
		{
			int likability = Random.Range(1, 11);
			
			if(likability == _Favorite)
			{
				destination = current;
				break;
			}
			
			if(likability > favorite)
			{
				favorite = likability;
				destination = current;
			}
		}
		
		return destination;
	}
	
	/// <summary>
	/// Rebuilds the list of connected waypoints.
	/// </summary>
	public void RebuildWaypointList()
	{
		Waypoints = (Waypoint[]) FindObjectsOfType(typeof(Waypoint));
		foreach(Waypoint point in Waypoints)
		{
			point.RecalculateConnectedWaypoints();
		}
	}
	
	/// <summary>
	/// Recalculates the connected waypoints for
	/// the current waypoint.
	/// </summary>
	public void RecalculateConnectedWaypoints()
	{
		Connected = new List<Waypoint>();
		
		foreach(Waypoint point in Waypoints)
		{
			if(point == this) { continue; }
			
			if(! Physics.Linecast(transform.position, point.transform.position))
			{
				Connected.Add(point);
			}
		}
	}
	
	#endregion Methods
}
