using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class WorldMapControl : MonoBehaviour, IPausableEntity 
{
	#region Constants
	
	private const float _moveEpsilon = 0.01f;
	
	#endregion Constants
	
	#region Variables
	
	public bool DebugMode = false;
	public float WalkSpeed = 2.0f;
	
	public List<DirectionalBehavior> Behaviors;
	
	public string IdlePart = "Idle";
	public string MovePart = "Move";
	public string UpPart = "Up";
	public string LeftPart = "Left";
	public string DownPart = "Down";
	public string RightPart = "Right";

	private bool _isPaused = false;
	private string _lastDirection;
	private string _animation;
	private SpriteSystem _sprite;
	private CharacterController _controller;
	
	#endregion Variables
	
	#region Engine Hooks
	
	public void Start()
	{
		_controller = GetComponent<CharacterController>();
		_sprite = GetComponentInChildren<SpriteSystem>();
		
		_lastDirection = DownPart;
	}
	
	public void Update()
	{
		if(_isPaused)
			return;

		DirectionalBehavior behavior = CheckInput();
		PerformMovement(behavior);
		Animate(behavior);
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private DirectionalBehavior CheckInput()
	{	
		Vector2 inputVector = ReadAxes();
		bool idle = inputVector == Vector2.zero;
		string actionModifier = idle ? IdlePart : MovePart;
		
		string directionModifier = ReadDirectionFromInputVector(inputVector);
		_lastDirection = directionModifier;
		
		DirectionalBehavior result = Behaviors.FirstOrDefault(b => b.HasState(actionModifier, directionModifier));
		if(result == default(DirectionalBehavior))
		{
			if(DebugMode)
				Debug.Log(string.Format("No Behavior exists for a {0} state facing {1}.", actionModifier, directionModifier));
		}
		
		return result;
	}
	
	private Vector2 ReadAxes()
	{
		return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
	}
	
	private string ReadDirectionFromInputVector(Vector2 inputVector)
	{
		string result = string.Empty;
		
		if(inputVector.x > 0)
			result = RightPart;
		else if(inputVector.x < 0)
			result = LeftPart;
					
		if(inputVector.y > 0)
			result = UpPart;
		else if(inputVector.y < 0)
			result = DownPart;
		
		if(string.IsNullOrEmpty(result))
			result = _lastDirection;
		
		return result;
	}
	
	private void PerformMovement(DirectionalBehavior behavior)
	{
		Vector3 moveVector = behavior.MoveDirection * WalkSpeed;
		moveVector.y = Physics.gravity.y;
		moveVector *= Time.deltaTime;
		
		_controller.Move(moveVector);
	}
	
	private void Animate(DirectionalBehavior behavior)
	{
		_sprite.PlaySingleFrame(behavior.ActionAnimation, false, AnimationMode.Loop);
	}

	public void PauseThisEntity()
	{
		_isPaused = true;
	}

	public void ResumeThisEntity()
	{
		_isPaused = false;
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
	
	#region Methods
	
	public bool HasState(string activity, string direction)
	{
		return Name.Contains(activity) && Name.Contains (direction);
	}
	
	#endregion Methods
}
