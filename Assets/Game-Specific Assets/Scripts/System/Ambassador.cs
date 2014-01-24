using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Programmer's Notes:
// --------------------------------------------------
// This is a very important class.  The Ambassador
// object is a persistent cache of player progress.
//
// When a player starts the game, this game object
// asserts its identity.  In addition to being able
// to load and save files, this class also handles
// keeping track of what items the player has gotten,
// and persisting data across scenes.
//
// Key to the item progress component is the
// ObtainableItem class.  This class is pretty
// much a serializable key/value pair that records
// whether, for some identifier, the player has
// already gotten it.
//
// Sequence Counters are a related idea.  Pretty
// much, a game's story can have multiple threads.
// based on a given thread's phase, we may want
// to show or hide certain entities, present
// certain NPC text, or other things.

[Serializable]
public class ObtainableItem
{
	public string Name;
	public bool Owned;
}

[Serializable]
public class SequenceCounter
{
	public string Name;
	public int Phase;
	public string QuestTitle;
	public string QuestDetails;

	public override string ToString()
	{
		return Name + " " + Phase 
			   + " (" + QuestTitle + ": " + QuestDetails + ")";
	}
}

public class Ambassador : ManagerMonoBehavior
{	
	#region Variables / Properties
	
	public int MaxHP = 8;
	public int Damage = 1;
	
	public List<SequenceCounter> SequenceCounters = new List<SequenceCounter>
	{
		new SequenceCounter{ Name = "Main", Phase = 0 }
	};
	
	public List<ObtainableItem> ItemList = new List<ObtainableItem>();
	
	public static Ambassador Instance 
	{
		get { return Ambassador.FindOldestInstance(); }
	}
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Use this for initialization
	void Awake () 
	{
		FindOldestInstance();
		SelfDestructIfOthersExist();	
		GameObject.DontDestroyOnLoad(gameObject);
	}
	
	IEnumerator OnLevelWasLoaded(int level)
	{
		FindOldestInstance();
		SelfDestructIfOthersExist();
		yield return SceneChangeManager.Instance.SetupPlayer();
	}
	
	#endregion Engine Hooks
	
	#region Base Class Overrides
	
	public override void SelfDestructIfOthersExist()
	{
		Ambassador[] objects = (Ambassador[]) FindObjectsOfType(typeof(Ambassador));
		IEnumerable<Ambassador> destructables = objects.Where(o => o.IsInitialInstance == false);
		
		if(DebugMode)
			Debug.LogWarning(destructables.Count() + " other managers were found!  Destroying them.");
		
		foreach(Ambassador current in destructables)
		{
			Destroy(current.gameObject);
		}
	}
	
	#endregion Base Class Overrides
	
	#region Methods
	
	public static Ambassador FindOldestInstance()
	{	
		Ambassador[] objects = (Ambassador[]) FindObjectsOfType(typeof(Ambassador));
		if(objects.Length == 0)
			throw new Exception("Ambassador could not find any ambassadors in the scene!");
		
		Ambassador oldest = objects.FirstOrDefault(a => a.IsInitialInstance == true);
		if(oldest == default(Ambassador))
		{
			oldest = objects.First();
			oldest.IsInitialInstance = true;
		}
		
		return oldest;
	}
	
	public void UpdateThread(string name, int progress, string questName = "", string questDetail = "")
	{
		SequenceCounter counter = SequenceCounters.FirstOrDefault(c => c.Name == name);
		if(counter == default(SequenceCounter))
		{
			if(DebugMode)
				Debug.LogWarning("Could not find Quest Thread " + name);
			
			return;
		}
		
		if(DebugMode)
			Debug.Log("Updating Quest Thread " + name + " to progress Lv." + progress);
		
		counter.Phase = progress;
		counter.QuestTitle = questName;
		counter.QuestDetails = questDetail;
	}
	
	public bool CheckThreadProgress(string name, int progress)
	{
		SequenceCounter arg = new SequenceCounter{ Name = name, Phase = progress };
		return CheckThreadProgress(arg);
	}
	
	public bool CheckThreadProgress(SequenceCounter requiredCounter)
	{
		SequenceCounter counter = SequenceCounters.FirstOrDefault(c => c.Name == requiredCounter.Name);
		if(counter == default(SequenceCounter))
		{
			if(DebugMode)
				Debug.LogWarning("Could not find quest thread " + name + "; definently NOT on that quest!");
			
			return false;
		}
		
		bool correctProgress = counter.Phase == requiredCounter.Phase;
		if(DebugMode)
			Debug.Log("The player " + (correctProgress ? "has" : "does not have") + " the right progress for quest " + name);
		
		return correctProgress;
	}
	
	public void GainItem(string name)
	{
		ObtainableItem item = ItemList.FirstOrDefault(i => i.Name == name);
		if(item == default(ObtainableItem))
		{
			if(DebugMode)
				Debug.LogWarning("Could not find item " + name + " in the listing; adding it as owned!");
			
			ItemList.Add(new ObtainableItem{Name = name, Owned = true});
			return;
		}
		
		if(DebugMode)
			Debug.Log("Adding ownership of item " + name);
		
		item.Owned = true;
	}
	
	public bool HasItem(string name)
	{
		ObtainableItem item = ItemList.FirstOrDefault(i => i.Name == name);
		if(item == default(ObtainableItem))
		{
			if(DebugMode)
				Debug.LogWarning("Could not find item " + name + " in the listing; the player dosen't have it!");
			
			return false;
		}
		
		return item.Owned;
	}
	
	#endregion Methods
}
