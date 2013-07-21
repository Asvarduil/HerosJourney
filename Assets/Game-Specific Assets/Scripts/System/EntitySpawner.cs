using UnityEngine;
using System.Collections;

public class EntitySpawner : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public PressureSwitch ActivationSwitch;
	public PressureSwitch ShutDownSwitch;
	public bool DisableOnSpawn = false;
	public float SpawnRate = 1.0f;
	public GameObject SpawnedEntity;
	
	private bool _isDisabled = false;
	private float _lastSpawn = 0.0f;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void FixedUpdate()
	{
		if(_isDisabled)
			return;
		
		if(! ActivationSwitch.IsPressed)
		{
			if(DebugMode)
				Debug.Log(ActivationSwitch.gameObject.name + " has not been pressed...");
			
			return;
		}
		
		if(ShutDownSwitch != null
		   && ShutDownSwitch.IsPressed)
		{
			if(DebugMode)
				Debug.Log(ShutDownSwitch.gameObject.name + " has been pressed.");
		
			return;
		}
		
		if(Time.time < _lastSpawn + SpawnRate)
		{
			if(DebugMode)
				Debug.Log("Not enough time has passed between spawns.");
			
			return;
		}
		
		Object spawned = GameObject.Instantiate(SpawnedEntity, transform.position, transform.rotation);
		if(DebugMode)
			Debug.Log (gameObject.name + " has just spawned a " + spawned.name + "(" + Time.time + ")");
		
		if(DisableOnSpawn)
			_isDisabled = true;
		
		_lastSpawn = Time.time;
	}
	
	#endregion Engine Hooks
}
