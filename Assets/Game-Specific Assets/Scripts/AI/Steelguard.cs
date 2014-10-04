using UnityEngine;
using System;
using System.Collections.Generic;

using Random = System.Random;

public class Steelguard : AIBase
{	
	#region Variables / Properties
	
	public float SwordDistance = 2f;
	public float DecisionRate = 1.5f;
	public float StrikeUnguardedRate = 25;
	public float MinimumHomingDistance = 0.05f;
	
	public string IdleLeft;
	public string IdleRight;
	public string HitLeft;
	public string HitRight;
	public string WalkLeft;
	public string WalkRight;
	public string AttackLeft;
	public string AttackRight;
	public string CrouchLeft;
	public string CrouchRight;
	public string CrouchAttackLeft;
	public string CrouchAttackRight;
	
	private bool _isFacingLeft = true;
	private string _currentAnimation;
	private float _nextAttack;
	private float _nextDecision;
	private AnimationMode _animationType = AnimationMode.Loop;
	
	private Vector3 _originalPosition;
	private StateMachine _states;
	private HitboxController _hitboxes;
	private SidescrollingMovement _movement;
	
	private bool _lockAnimation = false;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	// Use this for initialization
	public override void Start()
	{
		_originalPosition = transform.position;
		
		base.Start();
		_movement = GetComponent<SidescrollingMovement>();
		_hitboxes = GetComponentInChildren<HitboxController>();
		
		_states = new StateMachine(new List<State> {
			new State {Condition = DefaultCondition, Behavior = GuardOriginalPosition},
			new State {Condition = SeesPlayer, Behavior = MoveTowardPlayer},
			new State {Condition = InRange, Behavior = FenceWithPlayer},
			new State {Condition = PlayerIsJumping, Behavior = MoveTowardPlayer},
			new State {Condition = IsHit, Behavior = BeHit}
		});
	}
	
	// Update is called once per frame
	public void Update() 
	{
		if(_isPaused)
			return;
		
		_states.EvaluateState();

		_movement.PerformMovement();
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	public bool DefaultCondition() 
	{
		return true;
	}
	
	public bool SeesPlayer()
	{
		return _sense.DetectedPlayer;
	}
	
	public bool InRange()
	{
		if(_sense.PlayerLocation == null)
		{
			if(DebugMode)
				Debug.LogWarning("Did not detect any nearby players.");
			
			return false;
		}
		
		bool result = Vector3.Distance(transform.position, _sense.PlayerLocation.position) <= SwordDistance;
		if(DebugMode)
			Debug.Log("The enemy " + (result ? "is" : "is not") + " in range for me to attack!");
		
		return result;
	}
	
	public bool PlayerIsJumping()
	{
		if(_sense.PlayerState == null)
			return false;
		
		bool result = _sense.PlayerState.isJumping;
		if(DebugMode)
			Debug.Log("The enemy " + (result ? "is" : "is not") + " jumping.");
		
		return result;
	}
	
	public bool IsHit()
	{
		return _movement.MovementType == SidescrollingMovementType.Hit;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	public void GuardOriginalPosition()
	{
		if(Vector3.Distance(transform.position, _originalPosition) < MinimumHomingDistance)
		{
			BeIdle();
			return;
		}
		
		_lockAnimation = false;
		MoveTowardLocation(_originalPosition);
	}
	
	public void MoveTowardPlayer()
	{
		Vector3 target = _originalPosition;
		if(_sense.PlayerLocation != null)
		{
			if(DebugMode)
				Debug.Log("Found a player!");
			
			target = _sense.PlayerLocation.position;
		}
		else
		{
			if(DebugMode)
				Debug.LogWarning("Could not find a player!  Moving to original position instead.");
		}
			
		MoveTowardLocation(target);
	}
		
	public void FenceWithPlayer()
	{
		if(Time.time < _nextDecision)
			return;
			
		Random generator = new Random();
		int roll = generator.Next(0, 100);
		
		if(DebugMode)
			Debug.Log("Rolled " + roll + "; need " + StrikeUnguardedRate + " to attack the enemy's unguarded spot!");
		
		if(roll < StrikeUnguardedRate)
		{
			StrikeUnguardedArea();
		}
		else
		{
			StrikeGuardedArea();
		}
		
		_nextDecision = Time.time + DecisionRate;
		if(DebugMode)
			Debug.Log("Next decision at: " + _nextDecision);
	}
	
	#endregion Behaviors
	
	#region Methods
	
	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, _lockAnimation, _animationType);
		_hitboxes.PlaySingleFrame(_currentAnimation, _lockAnimation, _animationType);
	}
	
	private void StrikeUnguardedArea()
	{	
		if(!_sense.DetectedPlayer)
			return;
		
		bool shouldCrouch = ! _sense.PlayerState.isCrouching;
		if(DebugMode)
			Debug.Log((shouldCrouch ? "Crouching" : "Standing") + " to strike the player where they're not guarding!");
		
		_movement.ClearHorizontalMovement();
		PerformAttack(shouldCrouch);
	}
	
	private void StrikeGuardedArea()
	{	
		if(!_sense.DetectedPlayer)
			return;
		
		bool shouldCrouch = _sense.PlayerState.isCrouching;
		if(DebugMode)
			Debug.Log((shouldCrouch ? "Crouching" : "Standing") + " to strike the player where they are guarding!");
		
		_movement.ClearHorizontalMovement();
		PerformAttack(shouldCrouch);
	}
	
	private void BeHit()
	{
		_lockAnimation = true;
		_animationType = AnimationMode.OneShot;
		_currentAnimation = _isFacingLeft ? HitLeft : HitRight;
	}
	
	private void PerformAttack(bool shouldCrouch)
	{
		_lockAnimation = true;
		_animationType = AnimationMode.OneShot;
		_currentAnimation = shouldCrouch 
								? _isFacingLeft
									? CrouchAttackLeft
				                    : CrouchAttackRight
								: _isFacingLeft
									? AttackLeft
									: AttackRight;
	}
	
	private void MoveTowardLocation(Vector3 location)
	{
		_animationType = AnimationMode.Loop;
		
		if(Mathf.Abs(location.x - transform.position.x) < MinimumHomingDistance)
		{
			if(DebugMode)
				Debug.Log("Close enough, not moving.");
			
			return;
		}
		
		if(DebugMode)
			Debug.Log("Moving towards " + location);
		
		if(location.x < transform.position.x)
		{
			if(DebugMode)
				Debug.Log("Moving left...");
			
			_currentAnimation = WalkLeft;
			_isFacingLeft = true;
			_movement.MoveHorizontally(false);
			return;
		}
		else if(location.x > transform.position.x)
		{
			if(DebugMode)
				Debug.Log("Moving right...");
			
			_currentAnimation = WalkRight;
			_isFacingLeft = false;
			_movement.MoveHorizontally(true);
			return;
		}
	}
	
	private void BeIdle()
	{
		if(DebugMode)
			Debug.Log("I am being idle...");
		
		_lockAnimation = false;
		_movement.ClearHorizontalMovement();
		_currentAnimation = _isFacingLeft ? IdleLeft : IdleRight;
	}
	
	#endregion Methods
}
