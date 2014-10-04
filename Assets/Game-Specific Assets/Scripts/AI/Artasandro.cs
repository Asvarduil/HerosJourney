using UnityEngine;
using System;
using System.Collections.Generic;

public class Artasandro : AIBase
{
	#region Variables / Properties 

	public string IdleLeft;
	public string IdleRight;
	public string MoveLeft;
	public string MoveRight;
	public string AttackLeft;
	public string AttackRight;
	public string ChainCasting;
	
	public GameObject BlueMagicShot;

	public float TeleportDistance = 3f;
	public float AttackDistance = 0.95f;
	public GameObject TeleportEffect;

	private AnimationMode _animationType;
	private Vector3 _originalPosition;
	private Vector3 _spamEulerAngles;
	private Vector3 _moveDirection;
	private string _currentAnimation;
	private float _nextCastTime;
	private float _stopSpamTime;
	private bool _isFacingLeft;
	private int _aiPhase = 0;
	private List<Action> _actions;

	private Teleportation _teleportation;
	private SidescrollingMovement _movement;
	private SpiralProjectileSpam _spiralProjectileSpam;
	private HitboxController _hitboxes;

	#endregion Variables / Properties

	#region Engine Hooks

	public override void Start()
	{ 
		base.Start();

		_teleportation = GetComponent<Teleportation>();
		_movement = GetComponent<SidescrollingMovement>();
		_hitboxes = GetComponentInChildren<HitboxController>();
		_spiralProjectileSpam = GetComponent<SpiralProjectileSpam>();

		_actions = new List<Action>
		{
			MoveToPlayer,
			AttackPlayer,
			TeleportBehindPlayer,
			MoveToPlayer,
			AttackPlayer,
			TeleportToCenter,
			SpamMagic,
			TeleportBehindPlayer,
		};

		_originalPosition = transform.position;
		_currentAnimation = IdleLeft;
		_isFacingLeft = true;
	}

	public void FixedUpdate()
	{
		if(_isPaused)
			return;

		_actions[_aiPhase]();

		_movement.PerformMovement();
		PlayAnimations();
	}

	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, true, _animationType);
		_hitboxes.PlaySingleFrame(_currentAnimation, true, _animationType);
	}

	#endregion Engine Hooks

	#region Behaviors

	private void MoveToPlayer()
	{
		MoveTowardPlayer();

		if(InRange())
		{
			LogMessage("Within striking distance of player!");
			AdvanceToNextAiPhase();
		}
	}

	private void AttackPlayer()
	{
		_currentAnimation = _isFacingLeft
							? AttackLeft
							: AttackRight;

		if(_sprite.AnimationIsComplete())
		{
			LogMessage("Player has been attacked!");
			AdvanceToNextAiPhase();
		}
	}

	private void TeleportBehindPlayer()
	{
		Vector3 target = _sense.PlayerLocation.position;
		target.x += _sense.PlayerState.isFacingRight 
					? -TeleportDistance 
					: TeleportDistance;

		target = _teleportation.ConstraintPointToRectangle(target);
		_teleportation.TeleportToPoint(target);
		AdvanceToNextAiPhase();
	}

	private void TeleportToCenter()
	{
		_teleportation.TeleportToPoint(_originalPosition);

		_spiralProjectileSpam.ResetSpam();
		_currentAnimation = ChainCasting;
		AdvanceToNextAiPhase();
	}

	private void SpamMagic()
	{
		_movement.ClearHorizontalMovement();

		if(_spiralProjectileSpam.IsDoneSpamming)
		{
			LogMessage("Done spamming magic!  Time to attack.");
			AdvanceToNextAiPhase();
			return;
		}

		_spiralProjectileSpam.FireSomeSpam();
	}

	#endregion Behaviors

	#region Methods

	private bool InRange()
	{
		if(_sense.PlayerLocation == null)
		{
			LogMessage("Did not detect any nearby players.");
			return false;
		}
		
		bool result = Mathf.Abs(_sense.PlayerLocation.position.x - transform.position.x) < AttackDistance;
		LogMessage("The enemy " + (result ? "is" : "is not") + " in range for me to attack!");
		
		return result;
	}

	public void MoveTowardPlayer()
	{
		Vector3 target = _originalPosition;
		if(_sense.PlayerLocation != null)
		{
			LogMessage("Found a player!");
			target = _sense.PlayerLocation.position;
		}
		else
		{
			LogMessage("Could not find a player!  Moving to original position instead.");
		}
		
		MoveTowardLocation(target);
	}

	private void MoveTowardLocation(Vector3 location)
	{
		_animationType = AnimationMode.Loop;
		
		if(Mathf.Abs(location.x - transform.position.x) < AttackDistance)
		{
			LogMessage("Close enough, not moving.");
			return;
		}
		
		LogMessage("Moving towards " + location);
		
		if(location.x < transform.position.x)
		{
			_currentAnimation = MoveLeft;
			_isFacingLeft = true;
			_movement.MoveHorizontally(false);
			return;
		}
		else if(location.x > transform.position.x)
		{
			_currentAnimation = MoveRight;
			_isFacingLeft = false;
			_movement.MoveHorizontally(true);
			return;
		}
	}

	private void AdvanceToNextAiPhase()
	{
		_aiPhase++;
		if(_aiPhase > _actions.Count - 1)
		{
			_aiPhase = 0;
		}
	}

	#endregion Methods
}
