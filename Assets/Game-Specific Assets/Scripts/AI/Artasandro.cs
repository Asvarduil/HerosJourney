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

	public float MagicSpamTime = 10f;
	public float MagicSpamInterval = 0.75f;
	public float SpamRotateAngle = 30f;
	public GameObject BlueMagicShot;
	public GameObject QuadraMagicShot;

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

	private SidescrollingMovement _movement;
	private HitboxController _hitboxes;

	#endregion Variables / Properties

	#region Engine Hooks

	public override void Start()
	{
		base.Start();
		_movement = GetComponent<SidescrollingMovement>();
		_hitboxes = GetComponentInChildren<HitboxController>();

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

		PlayAnimations();
	}

	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation);
		_hitboxes.PlaySingleFrame(_currentAnimation);
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

		TeleportToPoint(target);
		AdvanceToNextAiPhase();
	}

	private void TeleportToCenter()
	{
		TeleportToPoint(_originalPosition);

		_stopSpamTime = Time.time + MagicSpamTime;
		_spamEulerAngles = Vector3.zero;
		//_currentAnimation = ChainCasting
		AdvanceToNextAiPhase();
	}

	private void SpamMagic()
	{
		_movement.ClearMovement();

		float currentTime = Time.time;
		if(currentTime >= _stopSpamTime)
		{
			LogMessage("Done spamming magic!  Time to attack.");
			AdvanceToNextAiPhase();
			return;
		}

		if(currentTime >= _nextCastTime)
		{
			GameObject.Instantiate(QuadraMagicShot, transform.position, Quaternion.Euler(_spamEulerAngles));
			_spamEulerAngles.z += SpamRotateAngle;
			_nextCastTime = currentTime + MagicSpamInterval;
		}
	}

	#endregion Behaviors

	#region Methods

	private void TeleportToPoint(Vector3 position)
	{
		LogMessage("Teleporting to " + position);

		GameObject.Instantiate(TeleportEffect, transform.position, Quaternion.identity);
		GameObject.Instantiate(TeleportEffect, _originalPosition, Quaternion.identity);
		
		transform.position = position;
	}

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
			if(DebugMode)
				Debug.Log("Moving left...");
			
			_currentAnimation = MoveLeft;
			_isFacingLeft = true;
			_movement.MoveLeft();
			return;
		}
		else if(location.x > transform.position.x)
		{
			if(DebugMode)
				Debug.Log("Moving right...");
			
			_currentAnimation = MoveRight;
			_isFacingLeft = false;
			_movement.MoveRight();
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
