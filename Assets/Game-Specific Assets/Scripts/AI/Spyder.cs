using UnityEngine;
using System;
using System.Collections.Generic;

public class Spyder : AIBase
{
	#region Variables / Properties
	
	public float MoveSpeed = 1;
	public float AttackSpeed = 2;
	public float AttackDelay = 0.5f;
	public float OverheadDetectTolerance = 0.75f;
	public string DefaultAnimation = "Default";
	public string MoveLeftAnimation = "Move Left";
	public string MoveRightAnimation = "Move Right";
	
	private bool _isAttacking = false;
	private bool _isDescendingWeb = false;
	private string _currentAnimation;
	
	private StateMachine _behaviorFSM;
	private StateMachine _animationFSM;
	private ProjectileProjector _projector;
	private CharacterController _controller;
	private LineRenderer _webThread;
	
	private Vector3 _originalLocation;
	
	#endregion Variables / Properties
	
	#region Engine Hooks

	// Use this for initialization
	public override void Start()
	{
		base.Start();
		_projector = GetComponentInChildren<ProjectileProjector>();
		_webThread = GetComponent<LineRenderer>();
		
		_controller = GetComponent<CharacterController>();
		
		_behaviorFSM = new StateMachine(new List<State>	{
			new State { Condition = DefaultCondition, Behavior = DoNothing},
			new State { Condition = SensesPlayer,     Behavior = MoveTowardPlayer },
			new State { Condition = IsOverPlayer,     Behavior = Attack },
			new State { Condition = AttackingTarget,  Behavior = VerticalGrappling }
		});
	}
	
	// Update is called once per frame
	protected void Update() 
	{
		if(_isPaused)
			return;
		
		_behaviorFSM.EvaluateState();
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Conditions
	
	public bool DefaultCondition()
	{
		return true;
	}
	
	public bool SensesPlayer()
	{
		bool result = _sense.DetectedPlayer;
		if(DebugMode && result)
			Debug.Log("I sense my prey nearby...");
		
		return result;
	}
	
	public bool IsOverPlayer()
	{
		if(! _sense.DetectedPlayer)
			return false;
		
		bool result = Mathf.Abs(_sense.PlayerLocation.position.x - transform.position.x) < OverheadDetectTolerance;
		if(DebugMode && result)
			Debug.Log("Prey is here!  I am poised to strike!");
		
		return result;
	}
	
	public bool AttackingTarget()
	{	
		return _isAttacking;
	}
	
	#endregion Conditions
	
	#region Behaviors
	
	public void DoNothing()
	{
		if(DebugMode)
			Debug.Log("Waiting for prey...");
		
		AnimateDefault();
		MoveWebLine();
	}
	
	public void MoveTowardPlayer()
	{	
		Vector3 direction;
		if(_sense.PlayerLocation.position.x < transform.position.x)
		{
			if(DebugMode)
				Debug.Log("Moving left, towards the player...");
			
			direction = new Vector3(-MoveSpeed, 0, 0);
			AnimateLeft();
		}
		else
		{
			if(DebugMode)
				Debug.Log("Moving right, towards the player...");
			
			direction = new Vector3(MoveSpeed, 0, 0);
			AnimateRight();
		}
		
		_controller.Move(direction * Time.deltaTime);
		MoveWebLine();
	}
	
	public void Attack()
	{
		if(DebugMode)
			Debug.Log("Feasting time...omnomnom...");
		
		_isAttacking = true;
		_isDescendingWeb = true;
		
		AnimateDefault();
		_projector.Fire();
	}
	
	public void VerticalGrappling()
	{
		Vector3 moveVector;
		if(_isDescendingWeb)
		{
			if(DebugMode)
				Debug.Log("I am climbing down the web...");
			
			moveVector = new Vector3(0, -AttackSpeed, 0);
		}
		else
		{
			if(DebugMode)
				Debug.Log("I am climbing up the web...");
			
			moveVector = new Vector3(0, AttackSpeed, 0);
		}
		
		CollisionFlags colInfo = _controller.Move(moveVector * Time.deltaTime);
		UpdateWebLine();
		
		if(_isDescendingWeb)
		{
			if((colInfo & CollisionFlags.Below) != CollisionFlags.None)
			{
				if(DebugMode)
					Debug.Log("I have struck something.  I must retreat to my ceiling!");
				
				_isDescendingWeb = false;
			}
		}
		else
		{
			if((colInfo & CollisionFlags.Above) != CollisionFlags.None)
			{
				if(DebugMode)
					Debug.Log("I have climbed back up.");
				
				_isAttacking = false;
				_isDescendingWeb = false;
			}
		}
	}
	
	#endregion Behaviors
	
	#region Methods
	
	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
	}
	
	public void MoveWebLine()
	{
		SetTopWebAnchor();
		UpdateWebLine();
	}
	
	public void SetTopWebAnchor()
	{
		_webThread.SetPosition(0, transform.position);
	}
	
	public void UpdateWebLine()
	{
		_webThread.SetPosition(1, transform.position);
	}
	
	public void AnimateLeft()
	{	
		_currentAnimation = MoveLeftAnimation;
	}
	
	public void AnimateRight()
	{	
		_currentAnimation = MoveRightAnimation;
	}
	
	public void AnimateDefault()
	{
		_currentAnimation = DefaultAnimation;
	}
	
	#endregion Methods
}
