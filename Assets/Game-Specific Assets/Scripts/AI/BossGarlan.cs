using UnityEngine;
using System;
using System.Collections.Generic;

public class BossGarlan : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string idleLeft;
	public string idleRight;
	public string attackLeft;
	public string attackRight;
	public string cast;
	public GameObject EarthShield;
	public GameObject Singularity;
	
	private string _currentAnimation;
	
	private StateMachine _fsm;
	private PlayerSense _sense;
	private SpriteSystem _sprite;
	private HitboxController _hitboxes;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start () 
	{
		_sense = GetComponentInChildren<PlayerSense>();
		_sprite = GetComponentInChildren<SpriteSystem>();
		_hitboxes = GetComponentInChildren<HitboxController>();

		_currentAnimation = idleRight;
		_fsm = new StateMachine(new List<State> {
			new State{ Condition = DefaultCondition, Behavior = DoNothing }
		});
	}
	
	public void Update () 
	{
		Action behavior = _fsm.EvaluateState();
		if(behavior != null)
			behavior();
		
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	private bool DefaultCondition()
	{
		return true;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	private void DoNothing()
	{
	}
	
	#endregion Behaviors
	
	#region Methods
	
	private void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
		_hitboxes.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
	}
	
	#endregion Methods
}
