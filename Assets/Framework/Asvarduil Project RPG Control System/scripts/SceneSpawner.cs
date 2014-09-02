using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class SceneSpawner : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start()
	{
		if(TransitionManager.Instance == null)
			throw new ArgumentNullException("TransitionManager.Instance", "TransitionManager must have its static instance initialized.");
		
		if(DebugMode)
		{
			Debug.Log ("Transition target scene: " + TransitionManager.Instance.targetSceneName 
				       + "(#" + TransitionManager.Instance.targetSceneID + ")" + Environment.NewLine
				       + "Transition Manager was created at: " + TransitionManager.Instance.CreateTime);
		}
		
		// If the manager is in a 'default' state, do not adjust the
		// player's position!
		if(TransitionManager.Instance.targetSceneID != -1 
		   || !string.IsNullOrEmpty(TransitionManager.Instance.targetSceneName))
		{
			AcquirePositionFromTransferManager();			
			LockCameraToPlayer();
		}
		else
		{
			if(DebugMode)
				Debug.Log("Target scene not found!");
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void AcquirePositionFromTransferManager()
	{
		if(DebugMode)
		Debug.Log("Target scene: " 
			      + TransitionManager.Instance.targetSceneName 
		    	  + " (" + TransitionManager.Instance.targetSceneID + ")");
		
		transform.position = TransitionManager.Instance.spawnPosition;
		transform.rotation = Quaternion.Euler(TransitionManager.Instance.spawnRotation);
	}
	
	private void LockCameraToPlayer()
	{
		GameObject player = GameObject.FindWithTag("Player");
		if(player == null)
			throw new ArgumentNullException("There is no Player-tagged game object in the scene!");
		
		var cam = (RPGCamera) GameObject.FindObjectOfType(typeof(RPGCamera));
		if(cam != null)
			cam.SetTarget(player);
		
		var sideCam = (SidescrollingCamera) GameObject.FindObjectOfType(typeof(SidescrollingCamera));
		if(sideCam != null)
			sideCam.TrackedEntity = player;
	}
	
	#endregion Methods
}

