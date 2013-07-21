using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class WorldMapControl : MonoBehaviour 
{
	#region Variables
	
	public float WalkSpeed = 2.0f;
	public List<DirectionalBehavior> Behaviors;
	
	public string UpName = "Up";
	public string LeftName = "Left";
	public string DownName = "Down";
	public string RightName = "Right";
	public string IdleName = "Idle";
	public string MoveName = "Move";
	
	private bool _idle = true;
	private string _animation;
	private string _direction;
	private Vector3 _moveVector;
	private SpriteSystem _sprite;
	private DirectionalBehavior _behavior;
	private CharacterController _controller;
	
	#endregion Variables
	
	#region Engine Hooks
	
	public void Start()
	{
		_controller = GetComponent<CharacterController>();
		_sprite = GetComponentInChildren<SpriteSystem>();
		
		_direction = DownName;
	}
	
	public void Update()
	{
		InitializeIdle();
		
		CheckHorizontalMovement();
		CheckVerticalMovement();
		
		PerformMovementAndAnimation();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void InitializeIdle()
	{
		_idle = true;
		_moveVector = Vector3.zero;
		_behavior = default(DirectionalBehavior);
	}
	
	public void CheckHorizontalMovement()
	{
		if(Input.GetAxis("Horizontal") < 0)
		{
			_idle = false;
			_direction = LeftName;
		}
		else if (Input.GetAxis("Horizontal") > 0)
		{
			_idle = false;
			_direction = RightName;
		}
	}
	
	public void CheckVerticalMovement()
	{
		if(Input.GetAxis("Vertical") > 0)
		{
			_idle = false;
			_direction = UpName;
		}
		else if (Input.GetAxis("Vertical") < 0)
		{
			_idle = false;
			_direction = DownName;
		}
	}
	
	public void PerformMovementAndAnimation()
	{
		string activityLevel = _idle 
						       ? IdleName 
							   : MoveName;
		_behavior = Behaviors.FirstOrDefault(b => b.Name.Contains(_direction)
				                                  && b.Name.Contains(activityLevel));
		
		if(_behavior != default(DirectionalBehavior))
		{
			_animation = _behavior.ActionAnimation;
			_moveVector = _behavior.MoveDirection * WalkSpeed;
			_moveVector.y = Physics.gravity.y;
		}
		
		_controller.Move(_moveVector * Time.deltaTime);
		_sprite.PlaySingleFrame(_animation, false, AnimationMode.Loop);
	}
	
	#endregion Methods
}

[Serializable]
public class DirectionalBehavior
{
	#region Variables / Properties
	
	public string Name;
	public Vector3 MoveDirection;
	public string ActionAnimation;
	
	#endregion Variables / Properties
}
