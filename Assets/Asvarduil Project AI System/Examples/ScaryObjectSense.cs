using UnityEngine;
using System;
using System.Collections.Generic;

public class ScaryObjectSense : MonoBehaviour 
{
	#region Variables / Properties
	
	/// <summary>
	/// Flag for whether this sense should output debug messages.
	/// </summary>
	public bool DebugMode = false;
	
	/// <summary>
	/// List of tags to be scared of.
	/// </summary>
	public List<string> ScaryTags;	
	
	/// <summary>
	/// List of known scary objects.
	/// </summary>
	public List<GameObject> ScaryObjects;
	
	/// <summary>
	/// Determines whether we know of a scary object.
	/// </summary>
	/// <value>
	/// <c>true</c> if this agent knows of a scary object; otherwise, <c>false</c>.
	/// </value>
	public bool IsScared 
	{ 
		get { return ScaryObjects.Count > 0; } 
	}
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		// Prep a new list of scary objects.
		ScaryObjects = new List<GameObject>();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		// Is the tag of the observed object scary?
		// If not, don't be scared.
		if(! ScaryTags.Contains(who.tag))
			return;
		
		// The object is scary.  We're frightened,
		// and we add the object to the list of
		// known scary objects.
		ScaryObjects.Add(who.gameObject);
		
		// If debugging, emit a message.
		if(DebugMode)
			Debug.Log(String.Format("Eeek!  {0} ({1}) appeared, I'm scared!", who.gameObject.name, who.gameObject.tag));
	}
	
	public void OnTriggerExit(Collider who)
	{
		// Is the tag of the observed object scary?
		// If not, not worth concern.
		if(! ScaryTags.Contains(who.tag))
			return;
		
		// The scary object can no longer be sensed.
		// Remove it from the list.
		ScaryObjects.Remove(who.gameObject);
		
		// If debugging, emit a message.
		if(DebugMode)
			Debug.Log(String.Format("Whew, {0} ({1}) is gone...and I'm {2} scared!", 
				                    who.gameObject.name, who.gameObject.tag, (IsScared ? "still" : "no longer")));
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	/// <summary>
	/// Unregisters the given scary object.
	/// </summary>
	/// <param name='scaryObject'>Supposedly scary object.</param>
	public void UnregisterScaryObject(GameObject scaryObject)
	{
		if(! ScaryObjects.Contains(scaryObject))
			return;
		
		if(DebugMode)
			Debug.Log(String.Format("Unregistering {0} per external command...", scaryObject.name));
		
		ScaryObjects.Remove(scaryObject);	
	}
	
	#endregion Methods
}
