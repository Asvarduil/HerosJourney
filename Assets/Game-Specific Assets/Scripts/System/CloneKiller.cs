using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class CloneKiller : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string CloneNameFragment = "(Clone)";
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnLevelWasLoaded()
	{
		TurnOffCloneProduction();
		RemoveAllClones();
	}
	
	public void OnApplicationQuit()
	{
		TurnOffCloneProduction();
		RemoveAllClones();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void TurnOffCloneProduction()
	{
		SpawnLoot.StopSpawningLoot();
	}
	
	public void RemoveAllClones()
	{
		if(DebugMode)
			Debug.LogWarning("Looking for all game objects with " + CloneNameFragment + " in their name, and destroying them!");
		
		IEnumerable<GameObject> allGameObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
		IEnumerable<GameObject> clones = allGameObjects.Where(o => o.name.Contains(CloneNameFragment));
		
		foreach(var currentClone in clones)
		{
			Destroy(currentClone);
		}
	}
	
	#endregion Methods
}
