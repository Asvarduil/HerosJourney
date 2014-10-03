using UnityEngine;
using System.Collections;

public class SidescrollingMovement : DebuggableBehavior
{
	#region Variables / Properties

	private const float _haltFallingEpsilon = 0.01f;
	
	public bool AllowHorizontalMovement = true;
	public bool AllowJumping = true;

	public float MoveSpeed = 5.0f;
	public int RemainingJumps = 1;
	public int MaximumAllowedJumps = 1;
	public float JumpForce = 20.0f;
	public float JumpLockout = 0.5f;
	public float JumpDecayRate = 0.5f;
	
	public bool IsFacingRight;
	public SidescrollingMovementType MovementType;

	private CharacterController _controller;

	private float _lastJump;
	private bool _isMovingThisFrame = false;
	private Vector3 _frameVelocity = Vector3.zero;
	private Vector3 _currentVelocity = Vector3.zero;
	
	public CollisionFlags CollisionDirection { get; private set; }
	
	public bool HitHead 
	{ 
		get 
		{ 
			return (CollisionDirection & CollisionFlags.CollidedAbove) != CollisionFlags.None; 
		} 
	}
	
	public bool TouchingWall
	{
		get
		{
			return (CollisionDirection & CollisionFlags.Sides) != CollisionFlags.None;
		}
	}
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_controller = GetComponent<CharacterController>();
	}
	
	#endregion Engine Hooks
	
	#region Messages
	
	public void TakeDamage(int damage)
	{
		MovementType = SidescrollingMovementType.Hit;
	}
	
	#endregion Messages
	
	#region Public Methods

	public void HaltMotion()
	{
		_currentVelocity = Vector3.zero;
	}

	public void PerformMovement()
	{	
		CheckIfGrounded();
		CheckIfHitHead();
		CheckIfFalling();
		
		CalculateFrameVelocity();
		ApplyVelocity();
	}

	public void RepelFromObject(GameObject thing, float repelForce)
	{
		Vector3 repelDirection = gameObject.transform.position - thing.transform.position;
		repelDirection = Vector3.Normalize(repelDirection) * repelForce;
		if(repelDirection.y == 0.0f)
			repelDirection.y = 19.6f;
		
		_isMovingThisFrame = true;
		_currentVelocity = repelDirection;
		ApplyVelocity();
	}

	public void ClearHorizontalMovement()
	{
		_isMovingThisFrame = MovementType != SidescrollingMovementType.Grounded;
		_currentVelocity.x = 0.0f;
	}

	private void CalculateFrameVelocity()
	{
		_frameVelocity = Vector3.zero;
		
		if(_isMovingThisFrame)
			_frameVelocity += _currentVelocity * Time.deltaTime;
		
		// Apply Gravity in a framerate-independent way...
		float gravityEffect = Physics.gravity.y * Time.deltaTime;
		_currentVelocity.y += gravityEffect;
		_frameVelocity.y += gravityEffect;
		
		// Reduce our next frame's jump velocity, such that we get a more parabolic flight.
		_currentVelocity.y *= JumpDecayRate;
		if(Mathf.Abs(_currentVelocity.y - 0.0f) < _haltFallingEpsilon)
			_currentVelocity.y = 0.0f;
		
		// Lock Z axis...
		_frameVelocity.z = 0;
	}

	private void ApplyVelocity()
	{
		CollisionDirection = _controller.Move(_frameVelocity);
	}

	private void CheckIfGrounded()
	{
		if(MovementType == SidescrollingMovementType.Grounded
		   || MovementType == SidescrollingMovementType.Jumping)
			return;
		
		if(! _controller.isGrounded)
			return;
		
		// Upon becoming grounded, refresh the total number of jumps.
		RemainingJumps = MaximumAllowedJumps;
		MovementType = SidescrollingMovementType.Grounded;
		_currentVelocity.y = 0;
	}
	
	private void CheckIfHitHead()
	{
		if(MovementType != SidescrollingMovementType.Jumping)
			return;
		
		if(! HitHead)
			return;

		MovementType = SidescrollingMovementType.Falling;
		_currentVelocity.y = 0;
	}
	
	private void CheckIfFalling()
	{
		if(MovementType == SidescrollingMovementType.Falling)
			return;
		
		if(! _controller.isGrounded
		   && MovementType == SidescrollingMovementType.Grounded)
		{
			MovementType = SidescrollingMovementType.Falling;
			return;
		}
		
		if(Mathf.Abs(_frameVelocity.y - 0.0f) < _haltFallingEpsilon
		   && MovementType == SidescrollingMovementType.Jumping)
		{
			MovementType = SidescrollingMovementType.Falling;
		}
	}

	public void MoveHorizontally(bool moveRight)
	{
		if(!AllowHorizontalMovement)
			return;

		IsFacingRight = moveRight;
		_isMovingThisFrame = true;
		_currentVelocity.x = moveRight ? MoveSpeed : -MoveSpeed;
	}

	public void Jump()
	{
		if(!AllowJumping)
			return;
		
		if(Time.time < _lastJump + JumpLockout)
			return;
		
		if(MovementType != SidescrollingMovementType.Grounded
		   && RemainingJumps == 0)
		{
			DebugMessage("The player is airborne, and unable to make any additional jumps.");
			return;
		}
		
		DebugMessage("The character has made a jump!");
		
		_isMovingThisFrame = true;
		_lastJump = Time.time;
		RemainingJumps--;
		MovementType = SidescrollingMovementType.Jumping;
		_currentVelocity.y = JumpForce;
	}
	
	public void HaltJump()
	{
		MovementType = SidescrollingMovementType.Falling;
		_currentVelocity.y = Mathf.Abs(Physics.gravity.y);
	}
	
	#endregion Public Methods
}
