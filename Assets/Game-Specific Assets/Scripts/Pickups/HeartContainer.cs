using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class HeartContainer : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public List<string> AffectedTags = new List<string>{"Player"};
	public int HealthIncrease = 2;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnTriggerEnter(Collider who)
	{
		if(! AffectedTags.Contains(who.tag))
			return;
		
		if(DebugMode)
			Debug.Log(who.gameObject.name + " picked up a Heart Container!");
		
		HealthSystem health = who.gameObject.GetComponent<HealthSystem>();
		health.IncreaseCapacity(HealthIncrease);
		
		Destroy(gameObject);
	}
	
	#endregion Engine Hooks
}
