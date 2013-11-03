using UnityEngine;
using System.Collections;

public class CameraFreezeTrigger : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool debugMode = false;
	public string tagToReactTo = "Player";
	private SidescrollingCamera _cam;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_cam = (SidescrollingCamera) FindObjectOfType(typeof(SidescrollingCamera));
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != tagToReactTo)
			return;
		
		if(debugMode)
			Debug.Log("A player has entered the cam freeze trigger.");
		
		_cam.FollowEntity = false;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != tagToReactTo)
			return;
		
		if(debugMode)
			Debug.Log("A player has left the cam freeze trigger.");
		
		_cam.FollowEntity = true;
	}
	
	#endregion Engine Hooks
}
