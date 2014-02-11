using UnityEngine;
using System;
using System.Collections;

public class SceneTeleporter : MonoBehaviour, IPausableEntity
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public string AffectedTag = "Player";
	public GUISkin Skin;	
	public FloatingButton EnterButton;
	public Vector3 ScenePosition;
	public GameObject TeleportEffect;
	
	private bool _showGUI = false;
	private bool _teleporterIsPaused = false;
	private GameObject _target;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnGUI()
	{
		if(! _showGUI
		   || _teleporterIsPaused)
			return;
		
		GUI.skin = Skin;
		if(EnterButton.IsClicked()
		   || Input.GetButtonUp("Interact"))
		{
			if(DebugMode)
				Debug.Log("Enter button was clicked for Teleporter: " + gameObject.name);
			
			CheckIfTransitionOccurred();
		}
	}
	
	public void Update()
	{		
		EnterButton.CalculatePosition(transform.position);
	}
	
	public void OnTriggerStay(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		_target = who.gameObject;
		_showGUI = true;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		_target = null;
		_showGUI = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void CheckIfTransitionOccurred()
	{	
		if(_target == null)
			throw new Exception("No game object to teleport!");
		
		GameObject.Instantiate(TeleportEffect, _target.transform.position, Quaternion.identity);
		GameObject.Instantiate(TeleportEffect, ScenePosition, Quaternion.identity);
		
		_target.transform.position = ScenePosition;
	}
	
	#endregion Methods
	
	#region Implementation of IPausableEntity
	
	public void PauseThisEntity()
	{
		if(DebugMode)
			Debug.Log("Pausing Door interface!");
		
		_teleporterIsPaused = true;
	}
	
	public void ResumeThisEntity()
	{
		if(DebugMode)
			Debug.Log("Resuming Door interface!");
		
		_teleporterIsPaused = false;
	}
	
	#endregion Implementation of IPausableEntity
}
