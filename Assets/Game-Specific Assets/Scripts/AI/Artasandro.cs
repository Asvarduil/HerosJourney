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

	private Vector3 _moveDirection;
	private Vector3 _middleCoordinates;
	private Vector3 _spamEulerAngles;
	private string _currentAnimation;
	private float _nextCastTime;
	private float _stopSpamTime;
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

		_middleCoordinates = transform.position;
		_currentAnimation = IdleLeft;
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
		LogMessage("Moving towards the player...");

		/*
		if(Vector3.Distance(_sense.PlayerLocation.position, transform.position) < AttackDistance)
		{
			LogMessage("Within striking distance of player!");
			AdvanceToNextAiPhase();
		}
		*/

		AdvanceToNextAiPhase();
	}

	private void AttackPlayer()
	{
		AdvanceToNextAiPhase();
	}

	private void TeleportBehindPlayer()
	{
		AdvanceToNextAiPhase();
	}

	private void TeleportToCenter()
	{
		LogMessage("Teleporting to " + _middleCoordinates + ", and spamming magic!");

		GameObject.Instantiate(TeleportEffect, transform.position, Quaternion.identity);
		GameObject.Instantiate(TeleportEffect, _middleCoordinates, Quaternion.identity);

		transform.position = _middleCoordinates;

		_stopSpamTime = Time.time + MagicSpamTime;
		_spamEulerAngles = Vector3.zero;
		//_currentAnimation = ChainCasting
		AdvanceToNextAiPhase();
	}

	private void SpamMagic()
	{
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
