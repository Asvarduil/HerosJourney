using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class BossGarlan : BossAI
{
	#region Enumerations
	
	public enum GarlanSequenceActions
	{
		None,
		Seek,
		Attack,
		Jump
	}
	
	#endregion Enumerations
	
	#region Variables / Properties
	
	public string idleLeft;
	public string idleRight;
	public string attackLeft;
	public string attackRight;
	public string cast;
	public float DecisionTime = 1.0f;
	public float ChargeHaltRange = 0.5f;
	
	private bool _isDamaged = false;
	private bool _isCharging = false;
	private bool _facingLeft = false;
	private float _nextDecision = 0.0f;
	private Vector3 _playerLocation;
	
	private GarlanSequenceActions _action = GarlanSequenceActions.None;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start () 
	{
		_currentAnimation = idleRight;
		
		// Link to components...
		_movement = GetComponent<SidescrollingMovement>();
		_sense = GetComponentInChildren<PlayerSense>();
		_sprite = GetComponentInChildren<SpriteSystem>();
		_hitboxes = GetComponentInChildren<HitboxController>();
		
		// Setup FSM...
		_states = new StateMachine(new List<State> {
			// Do nothing by default.
			new State(DefaultCondition, DefaultAction),
			
			// If has no combat action, roll one.
			new State(NeedsToChooseAction, RollNewAction),
			
			// If can't see player, look for them.
			new State(SeekingPlayer, SeekPlayer),
			
			// ...If decided to charge...
			new State(ChosenToCharge, ChargePlayer),
			
			// Must always be final, as it overrides all other actions.
			new State(IsDamaged, BeDamaged)
		});
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	private bool NeedsToChooseAction()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.None;
	}
	
	private bool SeekingPlayer()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.Seek;
	}
	
	private bool ChosenToCharge()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.Attack;
	}
	
	private bool IsDamaged()
	{
		return _isDamaged;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	private void RollNewAction()
	{
		// Are we still thinking about what to do?  If so,
		// keep thinking, and show off that idle pose.
		if(Time.time < _nextDecision)
		{
			DetermineDirection("Idle");
			return;
		}
		
		// When rolling, determine if I can still see the player.
		// If not, we must find him!
		if(! _sense.DetectedPlayer)
		{
			if(debugMode)
				Debug.Log("Garlan is seeking the player!");
			
			_action = GarlanSequenceActions.Seek;
			return;
		}
			
		// So, we've thought it over, and can see the player.
		// Decision time.
		_isDamaged = false;
		int roll = Random.Range(1, 100);
		switch(roll)
		{
			default:
				_action = GarlanSequenceActions.Attack;
				break;
		}
		
		if(debugMode)
			Debug.Log("Rolled decision: " + roll);
	}
	
	private void SeekPlayer()
	{
		TurnAroundIfFacingWall();	
		PerformMove();
		
		// If we can now see the player, time to decide what
		// we are going to do to him.
		if(_sense.DetectedPlayer)
		{
			if(debugMode)
				Debug.Log("Garlan has found the player!");
			
			_action = GarlanSequenceActions.None;
		}
	}
	
	private void ChargePlayer()
	{	
		if(! _isCharging)
		{
			_isCharging = true;
			FacePlayer();
			DetermineDirection("Attack");
		}
		
		PerformMove();
		
		// If close enough to where the player was, decide what to do next.
		if(Vector3.Distance(transform.position, _playerLocation) <= ChargeHaltRange)
		{
			_isCharging = false;
			_action = GarlanSequenceActions.None;
			_nextDecision = Time.time + DecisionTime;
		}
	}
	
	private void BeDamaged()
	{
		if(! _movement.isGrounded)
			return;
		
		if(debugMode)
			Debug.Log("Garlan is no longer damaged.");
		
		_action = GarlanSequenceActions.None;
		_isDamaged = false;
		_nextDecision = Time.time + DecisionTime;
	}
	
	#endregion Behaviors
	
	#region Methods
	
	private void TurnAroundIfFacingWall()
	{
		// If I'm touching a wall, he's not the direction I'm facing.
		// Turn around and move the other way.
		if(! _movement.TouchingWall)
			return;
		
		if(debugMode)
			Debug.Log("I'm facing a wall...guess he's not this way.");
		
		_facingLeft = !_facingLeft;
		DetermineDirection("Attack");
	}
	
	private void FacePlayer()
	{
		if(debugMode)
			Debug.Log("Charging the player!");
		
		_playerLocation = _sense.PlayerLocation.position;
		_facingLeft = _playerLocation.x < transform.position.x;
	}
	
	private void DetermineDirection(string motion)
	{
		string chargeDirection = _facingLeft ? "Left" : "Right";
		_currentAnimation = String.Format("{0}-{1}", motion, chargeDirection);
		
		if(debugMode)
			Debug.Log("Current animation: " + _currentAnimation);
	}
	
	private void PerformMove()
	{
		if(_facingLeft)
			_movement.MoveLeft();
		else
			_movement.MoveRight();
	}
	
	#endregion Methods
}
