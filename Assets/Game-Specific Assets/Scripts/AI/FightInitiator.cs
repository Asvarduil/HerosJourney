using UnityEngine;
using System.Collections;

public class FightInitiator : MonoBehaviour 
{
	#region Variables / Properties
	
	public BossAI Boss;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void InitiateFight()
	{
		Boss.FightStarted = true;
	}
	
	#endregion Methods
}
