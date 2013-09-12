using UnityEngine;
using System.Collections.Generic;

public class NPC : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string LeftAnimation = "Left";
	public string RightAnimation = "Right";
	
	private string _animation;
	private PlayerSense _sense;
	private SpriteSystem _sprite;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_sense = GetComponentInChildren<PlayerSense>();
		_sprite = GetComponentInChildren<SpriteSystem>();
	}
	
	public void FixedUpdate()
	{
		if(! _sense.DetectedPlayer)
			return;
		
		_animation = DetermineFaceDirection();
		_sprite.PlaySingleFrame(_animation, false, AnimationMode.Loop);
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public string DetermineFaceDirection()
	{
		if(_sense.PlayerLocation == null)
		{
			if(DebugMode)
				Debug.LogWarning("Player transform was not assigned by the player sensor!");
			
			return _animation;
		}
		
		if(_sense.PlayerLocation.position.x < transform.position.x)
		{
			if(DebugMode)
				Debug.Log("Player is to the left of me!  Facing that way.");
			
			return LeftAnimation;
		}
		
		if(DebugMode)
			Debug.Log("Player is to the right of me!  Facing that way.");
		
		return RightAnimation;
	}
	
	#endregion Methods
}
