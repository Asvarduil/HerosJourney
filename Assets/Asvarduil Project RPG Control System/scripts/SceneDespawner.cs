using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

// Programmer's Notes:
// -----------------------------------------
// This script exists to cause an element that
// the game's state is not ready for to pre-emptively
// despawn.

public class SceneDespawner : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public List<SequenceCounter> AllowedPhases;
	
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start() 
	{
		_ambassador = Ambassador.Instance;
		EvaluatePhase();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void EvaluatePhase()
	{
		if(_ambassador == null)
			throw new ArgumentNullException("_ambassador", "Ambassador must have an instance set up in order for phases to be evaluated.");
		
		bool shouldDestroy = ! DoesAmbassadorContainAnAllowedPhase();
		if(! shouldDestroy)
			return;
		
		if(DebugMode)
			Debug.Log("Destroying " + gameObject.name);
		
		Destroy(gameObject);
	}
	
	private bool DoesAmbassadorContainAnAllowedPhase()
	{
		bool result = false;
		foreach(SequenceCounter current in AllowedPhases)
		{
			Func<SequenceCounter, bool> existenceFilter = s => s.Name == current.Name 
				                                               && s.Phase == current.Phase;
			
			if(_ambassador.SequenceCounters.Any(existenceFilter))
			{
				result = true;
				break;
			}
		}
		
		return result;
	}
	
	#endregion Methods
}
