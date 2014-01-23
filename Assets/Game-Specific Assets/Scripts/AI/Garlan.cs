using UnityEngine;
using System;
using System.Collections.Generic;

using Random = System.Random;

public class Garlan : AIBase
{
	#region Variables / Properties
	
	public float SwordDistance = 2f;
	public float DecisionRate = 1.5f;
	public float StrikeUnguardedRate = 25;
	public float HomingDistance = 0.05f;
	public float TeleportDistance = 5.0f;
	public Rect RoomRectangle;
	
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
	public string Underthrust;

	private int _aiPhase = 0;
	private string _currentAnimation;
	private bool _hasAttacked = true;
	private bool _isFacingLeft = true;
	private AnimationMode _animationType;
	
	private List<Action> _actions;
	private Vector3 _originalPosition;
	private HitboxController _hitboxes;
	private Teleportation _teleportation;
	private PedestrianMovement _pedestrianMovement;

	#endregion Variables / Properties
	
	#region Engine Hooks

	public override void Start()
	{
		_originalPosition = transform.position;
		
		base.Start();
		_hitboxes = GetComponentInChildren<HitboxController>();
		_teleportation = GetComponent<Teleportation>();
		_pedestrianMovement = GetComponent<PedestrianMovement>();
		
		_actions = new List<Action>{
			MoveTowardsPlayer,
			AttackPlayer,
			TeleportBehindPlayer,
			MoveTowardsPlayer,
			AttackPlayer,
			TeleportAbovePlayer,
			UnderthrustTowardsPlayer
		};
	}

	public void Update() 
	{
		if(_isPaused)
			return;

		_actions[_aiPhase]();

		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Behaviors

	public void MoveTowardsPlayer()
	{
		_pedestrianMovement.MoveTowardLocation(_sense.PlayerLocation.position);

		_currentAnimation = _pedestrianMovement.IsFacingLeft
				? WalkLeft
				: WalkRight;

		if(_pedestrianMovement.IsCloseToTarget(_sense.PlayerLocation.position))
			AdvanceToNextAiPhase();
	}

	public void AttackPlayer()
	{
		_pedestrianMovement.BeStill();

		if(_hasAttacked)
			RollAttack();

		if(_sprite.AnimationIsComplete())
		{
			_hasAttacked = false;
			AdvanceToNextAiPhase();
		}
	}

	public void TeleportBehindPlayer()
	{
		_pedestrianMovement.BeStill();

		// Initial teleport amount...
		Vector3 teleportAmount = _sense.PlayerState.isFacingRight
				? new Vector3(-TeleportDistance, 0, 0)
				: new Vector3(TeleportDistance, 0, 0);
		Vector3 target = _sense.PlayerLocation.position += teleportAmount;
		target = Teleportation.ConstraintPointToRectangle(target, RoomRectangle);
		_teleportation.TeleportToPoint(target);

		AdvanceToNextAiPhase();
	}

	public void TeleportAbovePlayer()
	{
		_pedestrianMovement.BeStill();

		Vector3 teleportAmount = new Vector3(0, TeleportDistance, 0);
		Vector3 target = _sense.PlayerLocation.position += teleportAmount;
		target = Teleportation.ConstraintPointToRectangle(target, RoomRectangle);	  
		_teleportation.TeleportToPoint(target);

		AdvanceToNextAiPhase();
	}

	public void UnderthrustTowardsPlayer()
	{
		_pedestrianMovement.BeStill();
		_currentAnimation = Underthrust;

		if(_pedestrianMovement.IsCloseToTarget(_sense.PlayerLocation.position))
			AdvanceToNextAiPhase();
	}
	
	#endregion Behaviors
	
	#region Methods

	private void RollAttack()
	{
		Random generator = new Random();
		int roll = generator.Next(0, 100);
		
		if(DebugMode)
			Debug.Log("Rolled " + roll + "; need " + StrikeUnguardedRate + " to attack the enemy's unguarded spot!");
		
		if(roll < StrikeUnguardedRate)
			StrikeUnguardedArea();
		else
			StrikeGuardedArea();
	}

	private void StrikeUnguardedArea()
	{	
		if(!_sense.DetectedPlayer)
			return;
		
		bool shouldCrouch = ! _sense.PlayerState.isCrouching;
		if(DebugMode)
			Debug.Log((shouldCrouch ? "Crouching" : "Standing") + " to strike the player where they're not guarding!");

		PerformAttack(shouldCrouch);
	}
	
	private void StrikeGuardedArea()
	{	
		if(!_sense.DetectedPlayer)
			return;
		
		bool shouldCrouch = _sense.PlayerState.isCrouching;
		if(DebugMode)
			Debug.Log((shouldCrouch ? "Crouching" : "Standing") + " to strike the player where they are guarding!");

		PerformAttack(shouldCrouch);
	}

	private void PerformAttack(bool shouldCrouch)
	{
		_animationType = AnimationMode.OneShot;
		_currentAnimation = shouldCrouch 
			? _isFacingLeft
				? CrouchAttackLeft
				: CrouchAttackRight
			: _isFacingLeft
				? AttackLeft
				: AttackRight;
	}

	private void AdvanceToNextAiPhase()
	{
		_aiPhase++;
		if(_aiPhase > _actions.Count - 1)
			_aiPhase = 0;
	}

	#endregion Methods
}
