using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class SenseEntities : MonoBehaviour 
{
	#region Public Fields
	
	public bool SensedPlayer = false;
	public GameObject LastSeenPlayer;
	
	private List<Collider> observed = new List<Collider>();
	
	#endregion Public Fields
	
	#region Engine Hooks
	
	/// <summary>
	/// When a collider enters the trigger, add it to the list.
	/// If it's a player, set the SensedPlayer flag.
	/// </summary>
	/// <param name='who'>
	/// The collider that entered.
	/// </param>
	public void OnTriggerEnter(Collider who)
	{
		observed.Add(who);
		
		if(who.tag == "Player")
		{
			SensedPlayer = true;
			LastSeenPlayer = who.gameObject;
			
			Debug.Log ("A player entered the sensory trigger.");
		}
	}
	
	/// <summary>
	/// When a collider leaves the trigger, remove from the list.
	/// Then, determine if even one player is still seen.
	/// </summary>
	/// <param name='who'>
	/// The collider that left.
	/// </param>
	public void OnTriggerExit(Collider who)
	{
		observed.Remove(who);
		
		if(who.tag == "Player")
		{
			SensedPlayer = observed.Count(x => x.tag == "Player") > 0;
			
			if(! SensedPlayer)
			{
				LastSeenPlayer = null;
				Debug.Log ("The last player entity left the sensory trigger.");
			}
		}
	}
	
	#endregion Engine Hooks
}
