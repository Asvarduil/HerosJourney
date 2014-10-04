using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class Shototo : AIBase
{
	#region Variables / Properties
	
	public string faceLeft = "Face Left";
	public string faceRight = "Face Right";
	public string jumpLeft = "Jump Left";
	public string jumpRight = "Jump Right";
	public string hitLeft = "Hit Left";
	public string hitRight = "Hit Right";
	
	public float ShotDelay = 2.0f;
	public float JumpDelay = 2.0f;
	public float JumpOffset = 1.0f;
	public int ShotChance = 33;
	public int JumpChance = 33;
	public float ProjectileVelocity = 6.0f;
	
	private string _animation;
	private float _lastShot;
	private float _lastJump;
	private bool _facingLeft = true;
	
	private StateMachine _fsm;
	private ProjectileProjector _snout;
	private SidescrollingMovement _movement;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	public override void Start() 
	{
		base.Start();
		_snout = GetComponentInChildren<ProjectileProjector>();
		_movement = GetComponent<SidescrollingMovement>();
		
		List<State> states = new List<State>
		{
			new State{ Condition = () => true, Behavior = DoNothing },
			new State{ Condition = PlayerNearby, Behavior = ChooseAction },
			new State{ Condition = IsHit, Behavior = DoNothing }
		};
		
		_fsm = new StateMachine(states);
	}
	
	void Update() 
	{
		if(_isPaused)
			return;
		
		FacePlayer();
		_fsm.EvaluateState();

		_movement.PerformMovement();
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	public bool PlayerNearby()
	{
		return _sense.DetectedPlayer;
	}
	
	public bool IsHit()
	{
		return _movement.MovementType == SidescrollingMovementType.Hit;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	public void DoNothing()
	{
	}
	
	public void ChooseAction()
	{
		if(_sense.PlayerLocation.position.x < transform.position.x)
		{
			_facingLeft = true;
			_movement.MoveHorizontally(false);
		}
		else
		{
			_facingLeft = false;
			_movement.MoveHorizontally(true);
		}
		
		FireProjectile();
		DecideToJump();
	}
	
	#endregion Behaviors
	
	#region Methods
	
	private void FacePlayer()
	{
		if(_movement.MovementType == SidescrollingMovementType.Grounded)
		{
			_animation = _facingLeft ? faceLeft : faceRight;
		}
		else
		{
			_animation = _facingLeft ? jumpLeft : jumpRight;
		}
		
		if(_movement.MovementType == SidescrollingMovementType.Hit)
		{
			_animation = _facingLeft ? hitLeft : hitRight;
		}
	}
	
	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_animation, false, AnimationMode.Loop);
	}
	
	private void FireProjectile ()
	{
		int shotRoll = Random.Range(1, 100);
		if(shotRoll <= ShotChance
		   && Time.time >= _lastShot + ShotDelay)
		{		
			_snout.ProjectileVelocity = _facingLeft
				? new Vector3(-ProjectileVelocity, 0, 0)
				: new Vector3(ProjectileVelocity, 0, 0);
			_snout.Fire();
			_lastShot = Time.time;
		}
	}

	private void DecideToJump ()
	{
		int jumpRoll = Random.Range(1, 100);
		if(jumpRoll <= JumpChance
		   && Time.time >= _lastJump + JumpDelay)
		{		
			_movement.Jump();
			_lastJump = Time.time;
		}
	}
	
	#endregion Methods
}
