using UnityEngine;
using System.Collections.Generic;

public class SwitchComboActivator : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public GameObject AffectedObject;
	public List<PressureSwitch> Switches;
	public bool ActivateObject;

	private bool _allPressed = false;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Update()
	{
		CheckSwitches();
		EnableObject();
	}

	#endregion Engine Hooks

	#region Methods

	private void CheckSwitches()
	{
		// Problem - Detection logic not properly working.  Always returning false.
		foreach(PressureSwitch thisSwitch in Switches)
		{
			_allPressed &= thisSwitch.IsPressed;
		}
	}

	private void EnableObject()
	{
		if(_allPressed)
		{
			if(DebugMode)
				Debug.Log("All switches associated to " + gameObject.name + " have been pressed!");

			if(ActivateObject)
				AffectedObject.SetActive(true);
			else
				AffectedObject.SetActive(false);
		}
		else
		{
			if(DebugMode)
				Debug.Log("At least one switch associated to " + gameObject.name + " has not been pressed.");
		}
	}

	#endregion Methods
}
