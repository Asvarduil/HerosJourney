using UnityEngine;
using System.Collections;

public class WindBlock : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string SkillName = "Perseus Boots";
	
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	// Use this for initialization
	void Start () 
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		DeleteIfCannotUse();		
	}
	
	#endregion Engine Hooks
	
	#region Method
	
	private void DeleteIfCannotUse()
	{
		if(_ambassador == null)
		{
			if(DebugMode)
				Debug.LogWarning("An ambassador was not found!");
			
			Destroy(gameObject);
			return;
		}
		
		if(! _ambassador.HasItem(SkillName))
		{
			if(DebugMode)
				Debug.Log("Player does not have " + SkillName + ", not displaying Wind Blocks.");
			
			Destroy(gameObject);
		}
	}
	
	#endregion Methods
}
