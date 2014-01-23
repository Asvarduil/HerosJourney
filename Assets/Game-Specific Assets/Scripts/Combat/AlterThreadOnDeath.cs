using UnityEngine;

public class AlterThreadOnDeath : MonoBehaviour 
{
	#region Variables / Properties
	
	public string ThreadName;
	public int NewPhase;
	public string QuestTitle;
	public string QuestDetails;
	
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start()
	{
		_ambassador = Ambassador.FindOldestInstance();
	}
	
	void OnDestroy()
	{
		_ambassador.UpdateThread(ThreadName, NewPhase, QuestTitle, QuestDetails);
	}
	
	#endregion Engine Hooks
}
