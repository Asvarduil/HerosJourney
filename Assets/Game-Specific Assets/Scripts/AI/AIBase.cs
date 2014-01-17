using UnityEngine;
using System.Collections;

public abstract class AIBase : MonoBehaviour, IPausableEntity
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	
	protected bool _isPaused = false;
	protected PlayerSense _sense;
	protected SpriteSystem _sprite;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public virtual void Start()
	{
		_sense = GetComponentInChildren<PlayerSense>();
		_sprite = GetComponentInChildren<SpriteSystem>();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public virtual void PlayAnimations() 
	{	
	}

	protected void LogMessage(string message)
	{
		if(!DebugMode)
			return;

		Debug.Log(message);
	}
		
	#endregion Methods
	
	#region Implementation of IPausableEntity
	
	public void PauseThisEntity()
	{
		if(DebugMode)
			Debug.Log("Pausing AI execution!");
		
		_isPaused = true;
	}
	
	public void ResumeThisEntity()
	{
		if(DebugMode)
			Debug.Log("Resuming AI execution!");
		
		_isPaused = false;
	}
	
	#endregion Implementation of IPausableEntity
}
