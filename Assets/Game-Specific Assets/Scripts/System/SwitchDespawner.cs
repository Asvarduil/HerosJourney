using UnityEngine;
using System;

public class SwitchDespawner : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public PressureSwitch Switch;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		if(DebugMode)
			Debug.Log(String.Format("{0} is observing switch {1}...", gameObject.name, Switch.gameObject.name));
	}
	
	public void FixedUpdate()
	{
		if(Switch == null)
			return;

		if(! Switch.IsPressed)
			return;
		
		if(DebugMode)
			Debug.Log(String.Format("{0} was pressed.  Despawning {1}!", Switch.gameObject.name, gameObject.name));

		gameObject.SetActive(false);
	}
	
	#endregion Engine Hooks
}
