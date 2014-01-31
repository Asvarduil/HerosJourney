using UnityEngine;
using System.Collections.Generic;

public class ProjectileGroup : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public int ProjectileCount;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Start()
	{
		CheckProjectileCount();
	}

	public void Update()
	{
		CheckProjectileCount();
		if(ProjectileCount == 0)
		{
			if(DebugMode)
				Debug.Log(gameObject.name + " has no more children; self-destructing.");

			Destroy(gameObject);
		}
	}

	#endregion Engine Hooks

	#region Methods

	private void CheckProjectileCount()
	{
		ProjectileCount = transform.childCount;
	}

	#endregion Methods
}
