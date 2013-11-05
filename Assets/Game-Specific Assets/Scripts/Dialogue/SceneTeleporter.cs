using UnityEngine;
using System;
using System.Collections;

public class SceneTeleporter : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = true;
	public string AffectedTag = "Player";
	public GUISkin Skin;	
	public FloatingButton EnterButton;
	public Vector3 ScenePosition;
	public GameObject TeleportEffect;
	
	private GameObject _target;
	private bool _showGUI = false;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnGUI()
	{
		if(! _showGUI)
			return;
		
		GUI.skin = Skin;
		if(EnterButton.IsClicked())
		{
			if(DebugMode)
				Debug.Log("Enter button was clicked for Door: " + gameObject.name);
			
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
}
