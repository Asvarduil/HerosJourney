using UnityEngine;
using System.Collections;

public class PlayerSense : MonoBehaviour 
{
	#region Variables / Properties
	
	public string AffectedTag = "Player";
	public float SenseDelay = 1.0f;
	
	public bool DetectedPlayer;
	public Transform PlayerLocation;
	public PlayerControl PlayerState;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		DetectedPlayer = true;
		PlayerLocation = who.gameObject.transform;
		PlayerState = who.gameObject.GetComponent<PlayerControl>();
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		DetectedPlayer = false;
		PlayerLocation = null;
		PlayerState = null;
	}
	
	#endregion Methods	
}
