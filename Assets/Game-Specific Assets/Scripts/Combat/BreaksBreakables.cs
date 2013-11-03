using UnityEngine;
using System;
using System.Collections.Generic;

// Programmer's Notes:
// ---------------------------------------------
// Breakable objects are no different than any
// enemy, except they have no AI and can be
// struck a number of times before being destroyed.
//
// This script loads up information from the
// Ambassador, and passes it to the DamageSource
// on the same object.  If the player possesses
// a certain something, their DamageSource will
// have the list of breakable colliders added
// to colliders that it affects, thus causing
// those objects to have TakeDamage messages
// sent to them on weapon hit.

public class BreaksBreakables : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string ItemName;
	public List<string> BreakableItemTags;
	
	public bool CanBreakBreakables
	{
		get { return _ambassador.HasItem(ItemName); }
	}
	
	private DamageSource _damageSource;
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		_damageSource = GetComponent<DamageSource>();
		
		if(_ambassador == null
		   || _damageSource == null)
		{
			if(DebugMode)
				Debug.LogWarning("Was unable to find damage source or ambassador!" + Environment.NewLine
					             + "Ambassador: " + ((_ambassador == null) ? "Not Found" : "Found") + Environment.NewLine
					             + "Damage Source: " + ((_damageSource == null) ? "Not Found" : "Found"));
			
			return;
		}
		
		if(CanBreakBreakables)
		{
			if(DebugMode)
				Debug.Log("This damage trigger can break breakable objects.");
			
			_damageSource.AffectedTags.AddRange(BreakableItemTags);
		}
		else
		{
			if(DebugMode)
				Debug.Log("This damage trigger can't break breakables.");
		}
	}
	
	#endregion Engine Hooks
}
