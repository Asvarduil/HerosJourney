using UnityEngine;
using System;
using System.Collections.Generic;
using Random = System.Random;

public class Slime : AIBase
{
	#region Variables / Properties
	
	public string moveLeft = "Walk (Left)";
	public string moveRight = "Walk (Right)";
	public string attackLeft = "Attack Left";
	public string attackRight = "Attack Right";
	public string hitLeft = "Hit Left";
	public string hitRight = "Hit Right";
	
	public int attackChance = 20;
	public int fleeChance = 10;
	public float thinkDelay = 1;
	
	private bool _facingLeft = true;
	private bool? _isFleeing = null;
	private bool? _isAttacking = null;
	private bool? _isHit = null;
	private float _lastThought;
	private string _animation;
	
	private StateMachine _states;
	private SidescrollingMovement _movement;
	private HitboxController _hitboxController;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Start()
	{
		base.Start();
		_movement = GetComponent<SidescrollingMovement>();
		
		List<State> machineStates = new List<State>{
			new State{Condition = () => true, Behavior = Patrol},
			new State{Condition = SensePlayer, Behavior = FightOrFlight},
			new State{Condition = IsHit, Behavior = BeHit}
		};
		
		_states = new StateMachine(machineStates);
	}
	
	public void Update()
	{
		if(_isPaused)
			return;
		
		_states.EvaluateState();
		_movement.PerformMovement();
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Condition Methods
	
	private bool SensePlayer()
	{
		bool sensedPlayer = _sense.DetectedPlayer;
		return sensedPlayer;
	}
	
	private bool IsHit()
	{
		return _movement.MovementType == SidescrollingMovementType.Hit;
	}
	
	#endregion Condition Methods
		
	#region Behavior Methods
		
	private void Patrol()
	{	
		_movement.ClearHorizontalMovement();
		_animation = string.Empty;
		
		ChangeDirections();
		MoveInDirection();
	}
	
	private void FightOrFlight()
	{
		if(_isFleeing != null)
			return;
		
		if(Time.time < _lastThought + thinkDelay)
			return;
		
		_lastThought = Time.time;
		_movement.ClearHorizontalMovement();
		
		Random generator = new Random((int) DateTime.Now.Ticks);
		int roll = generator.Next(0, 99);
		
		// Default is for the slime to do their 'threatening wiggle'...
		_animation = _facingLeft ? moveLeft : moveRight;
		
		if(roll >= attackChance)
		{
			_isFleeing = false;
			_isAttacking = true;
			_facingLeft = !(_sense.PlayerLocation.position.x > transform.position.x);
			
			MoveInDirection();
			_movement.Jump();
			
			_animation = _facingLeft ? attackLeft : attackRight;
		}
		
		if(roll <= fleeChance)
		{
			_isFleeing = true;
			_isAttacking = false;
			
			_facingLeft = _sense.PlayerLocation.position.x > transform.position.x;
			MoveInDirection();
		}
	}
	
	public void BeHit()
	{
		_isFleeing = false;
		_isAttacking = false;
		
		_animation = _facingLeft ? hitLeft : hitRight;
	}
	
	#endregion Behavior Methods
	
	#region Methods
	
	private void ChangeDirections()
	{
		_isFleeing = null;
		
		if(_movement.TouchingWall)
			_facingLeft = !_facingLeft;
	}
	
	public void MoveInDirection()
	{
		if(_facingLeft)
		{
			_animation = moveLeft;
			_movement.MoveHorizontally(false);
		}
		else
		{
			_animation = moveRight;
			_movement.MoveHorizontally(true);
		}
	}
	
	public override void PlayAnimations()
	{
		bool lockAnimation = (_isAttacking == true) 
			                 || (_isHit == true);
		
		_sprite.PlaySingleFrame(_animation, lockAnimation, AnimationMode.Loop);
	}
	
	#endregion Methods
}
