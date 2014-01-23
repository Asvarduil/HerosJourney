using UnityEngine;
using System.Collections;

public class PedestrianMovement : MonoBehaviour 
{
	#region Variables / Properties

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
			return;

		if(location.x < transform.position.x)
		{
			IsFacingLeft = true;
			_movement.MoveLeft();
			return;
		}
		else if(location.x > transform.position.x)
		{
			IsFacingLeft = false;
			_movement.MoveRight();
			return;
		}
	}

	public bool IsCloseToTarget(Vector3 location) 
	{
		return Mathf.Abs(location.x - transform.position.x) < AttackDistance;
	}

	#endregion Actions

}
