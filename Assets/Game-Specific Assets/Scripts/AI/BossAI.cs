using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class BossAI : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public bool FightStarted = false;
	
	protected string _currentAnimation;
	
	protected StateMachine _fsm;
	protected PlayerSense _sense;
	protected SpriteSystem _sprite;
	protected HitboxController _hitboxes;
	protected SidescrollingMovement _movement;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Update () 
	{
		Action behavior = _fsm.EvaluateState();
		if(behavior != null)
			behavior();
		
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	protected virtual bool DefaultCondition()
	{
		return true;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	protected virtual void DefaultAction()
	{
	}
	
	#endregion Behaviors
	
	#region Methods
	
	protected virtual void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
		_hitboxes.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
	}
	
	#endregion Methods
}
