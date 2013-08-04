using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class BossGarlan : MonoBehaviour 
{
	#region Enumerations
	
	public enum GarlanSequenceActions
	{
		None,
		Attack,
		Cast
	}
	
	#endregion Enumerations
	
	#region Variables / Properties
	
	public bool DebugMode = false;
	public bool FightStarted = false;
	public string idleLeft;
	public string idleRight;
	public string attackLeft;
	public string attackRight;
	public string cast;
	public float ChargeHaltRange = 0.9f;
	public float SingularityCastTime;
	public GameObject EarthShield;
	public GameObject Singularity;
	
	public Dictionary<GarlanSequenceActions, int> StateChangeChances;
	
	private bool _isDamaged = false;
	private bool _facingLeft = false;
	private bool _castingSingularity = false;
	private float _spellDoneTime;
	private string _currentAnimation;
	private Vector3 _playerLocation;
	
	private GarlanSequenceActions _action = GarlanSequenceActions.None;
	private StateMachine _fsm;
	private PlayerSense _sense;
	private SpriteSystem _sprite;
	private HitboxController _hitboxes;
	private SidescrollingMovement _movement;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start () 
	{
		_movement = GetComponent<SidescrollingMovement>();
		
		_sense = GetComponentInChildren<PlayerSense>();
		_sprite = GetComponentInChildren<SpriteSystem>();
		_hitboxes = GetComponentInChildren<HitboxController>();

		_currentAnimation = idleRight;
		_fsm = new StateMachine(new List<State> {
			new State{ Condition = DefaultCondition,    Behavior = DoNothing },
			new State{ Condition = NeedsToChooseAction, Behavior = RollNewAction },
			new State{ Condition = ChosenToCharge,      Behavior = ChargePlayer },
			new State{ Condition = ChosenToCast,        Behavior = SummonBarrier },
			new State{ Condition = CastingSingularity,  Behavior = CastSingularity }
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
	
	private bool NeedsToChooseAction()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.None;
	}
	
	private bool ChosenToCharge()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.Attack;
	}
	
	private bool ChosenToCast()
	{
		return FightStarted
			   && _action == GarlanSequenceActions.Cast;
	}
	
	private bool CastingSingularity()
	{
		return _castingSingularity;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	private void DoNothing()
	{
	}
	
	private void RollNewAction()
	{
		_isDamaged = false;
		
		int roll = Random.Range(1, 100);
		foreach(KeyValuePair<GarlanSequenceActions, int> relationship in StateChangeChances)
		{
			if(relationship.Value > 0
			   && roll <= relationship.Value)
			{
				_action = relationship.Key;
				return;
			}
		}
	}
	
	private void ChargePlayer()
	{
		if(_playerLocation == Vector3.zero)
		{
			_playerLocation = _sense.PlayerLocation.position;
			_facingLeft = _playerLocation.x < transform.position.x;
			string chargeDirection = _facingLeft ? "Left" : "Right";
			_currentAnimation = String.Format("Charge-{0}", chargeDirection);
		}
		
		if(_facingLeft)
			_movement.MoveLeft();
		else
			_movement.MoveRight();
		
		if(Vector3.Distance(transform.position, _playerLocation) <= ChargeHaltRange)
		{
			_playerLocation = Vector3.zero;
			_action = GarlanSequenceActions.None;
		}
	}
	
	private void SummonBarrier()
	{
		Instantiate(EarthShield, transform.position, transform.rotation);
		
		_spellDoneTime = Time.time + SingularityCastTime;
		_currentAnimation = cast;
		_castingSingularity = true;
		_playerLocation = _sense.PlayerLocation.position;
	}
	
	private void CastSingularity()
	{
		if(Time.time < _spellDoneTime)
			return;
		
		Instantiate(Singularity, _playerLocation, Quaternion.Euler(Vector3.zero));
		
		_playerLocation = Vector3.zero;
		_castingSingularity = false;
		_action = GarlanSequenceActions.None;
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
