using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class SpawnLoot : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool CanSpawnItems = true;
	public List<LootTableItem> PossibleLoots;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnDestroy()
	{
		if(! CanSpawnItems)
			return;
		
		GameObject dropLoot = null;
		foreach(var current in PossibleLoots)
		{
			GameObject loot = current.RollForLoot();
			if(loot != null)
				dropLoot = loot;	
		}
		
		if(dropLoot == null)
			return;
		
		GameObject.Instantiate(dropLoot, transform.position, Quaternion.Euler(Vector3.zero));	
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public static void StopSpawningLoot()
	{	
		IEnumerable<SpawnLoot> lootSpawners = (SpawnLoot[]) GameObject.FindObjectsOfType(typeof(SpawnLoot));
		foreach(var currentSpawner in lootSpawners)
		{
			currentSpawner.CanSpawnItems = false;
		}
	}
	
	#endregion Methods
}

[Serializable]
public class LootTableItem
{
	#region Variables / Properties
	
	public GameObject Loot;
	public int Chance = 1;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public GameObject RollForLoot()
	{
		int roll = Random.Range(1, 100);	
		return (roll <= Chance)
			? Loot
			: null;
	}
	
	#endregion Methods
}
