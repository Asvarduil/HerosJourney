using UnityEngine;
using System.Collections;

public class PedestrianMovement : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public bool IsFacingLeft = true;
	public float AttackDistance = 1.0f;

	private SidescrollingMovement _movement;

	#endregion Variables / Properties

	#region Engine Hooks

	private void Start()
	{
		_movement = GetComponent<SidescrollingMovement>();
	}

	#endregion Engine Hooks

	#region Actions

	public void BeStill()
	{
		_movement.ClearMovement();
	}

	public void MoveTowardLocation(Vector3 location)
	{
		if(IsCloseToTarget(location))
		{
			if(DebugMode)
				Debug.Log("Close enough to " + location + ", don't need to move.");
			return;
		}

		if(location.x < transform.position.x)
		{
			IsFacingLeft = true;
			_movement.MoveLeft();
		}
		else if(location.x > transform.position.x)
		{
			IsFacingLeft = false;
			_movement.MoveRight();
		}

		if(DebugMode)
			Debug.Log("(" + Time.time + ") Moving " + (IsFacingLeft ? "Left" : "Right") + " from " + transform.position);
	}

	public bool IsCloseToTarget(Vector3 location) 
	{
		return Mathf.Abs(location.x - transform.position.x) < AttackDistance;
	}

	#endregion Actions

}
