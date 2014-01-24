using UnityEngine;
using System.Collections;

public class SkillPickup : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public bool ShouldSelfDestruct = false;
	public string SkillName;
	public string AffectedTag = "Player";
	
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	void Start () 
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		
		if(_ambassador == null)
		{
			if(DebugMode)
				Debug.LogWarning("An ambassador was not found!");
			
			return;
		}
		
		if(_ambassador.HasItem(SkillName))
			RemoveItemFromScene();
	}
	
	void OnTriggerEnter(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		if(_ambassador == null)
		{
			if(DebugMode)
				Debug.LogWarning("Doing nothing, because no ambassador was found.");
			
			return;
		}
		
		GiveSkillToFinder();
		RemoveItemFromScene();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void GiveSkillToFinder()
	{
		if(DebugMode)
			Debug.Log("The player will learn " + SkillName);
		
		_ambassador.GainItem(SkillName);
	}
	
	private void RemoveItemFromScene()
	{
		if(! ShouldSelfDestruct)
			return;

		if(DebugMode)
			Debug.Log("Removing " + gameObject.name + " from the scene.");
		
		Destroy (gameObject);
	}
	
	#endregion Methods
}
