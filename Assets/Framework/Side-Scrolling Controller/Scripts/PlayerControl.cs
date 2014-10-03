using UnityEngine;
using System.Collections;

public class PlayerControl : DebuggableBehavior, IPausableEntity 
{
	#region Variables / Properties
	
	public AudioClip attackSound;
	public AudioClip jumpSound;

	public bool allowAction = true;
	public bool canAttack = true;
	public bool canOverthrust = false;
	public bool canUnderthrust = false;

	public PlayerControlState controlState = PlayerControlState.IdleRight;

	// TODO: Refactor to be private, AI looks at controlState instead.
	public bool isHit = false;
	public bool isJumping = false;
	public bool isCrouching = false;
	public bool isAttacking = false;
	public bool isFacingRight = true;
	public bool isOverthrusting = false;
	public bool isUnderthrusting = false;
	public bool isMovingHorizontally = false;

	public float AttackLockout = 0.25f;

	private float _lastAttack;
	private string _currentSequence;
	
	private SidescrollingMovement _movement;
	private HitboxController _hitboxController;
	private PlayerAnimationManager _animation;
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start() 
	{
		_maestro = (Maestro) FindObjectOfType(typeof(Maestro));
		_movement = gameObject.GetComponent<SidescrollingMovement>();
		_animation = GetComponent<PlayerAnimationManager>();
	}
	
	public void Update()
	{	   	
		ProcessAxes();
		DetermineControlState();
        
		_movement.PerformMovement();
		_animation.PerformAnimation();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void Halt()
	{
		canAttack = false;
		isJumping = false;
		allowAction = false;
		isAttacking = false;
		isCrouching = false;
		isOverthrusting = false;
		isUnderthrusting = false;
		isMovingHorizontally = false;

		_movement.HaltJump();
		_movement.HaltMotion();
	}
	
	public void Resume()
	{
		canAttack = true;
		allowAction = true;
	}

	private void ProcessAxes()
	{
		if(! allowAction)
			return;

		CheckCrouching();
        CheckHorizontal();
		CheckJump();
		CheckGrounded();

		CheckAttack();
		CheckOverthrust();
		CheckUnderthrust();

		CheckInjured();
	}

	private void CheckGrounded()
	{
		// If grounded and no jump occurred, clear the jump flag.
		if(_movement.MovementType != SidescrollingMovementType.Grounded)
			return;

		isJumping = false;
		isHit = false;
	}

	private void CheckJump()
	{		
		if(Input.GetButtonUp("Jump"))
		{
			if(_movement.MovementType == SidescrollingMovementType.Jumping)
			{
				isJumping = false;
				_movement.HaltJump();
			}
		}
		else if(Input.GetButtonDown("Jump"))
		{
			if(_movement.MovementType == SidescrollingMovementType.Grounded)
			{
				DebugMessage("Player is jumping!");
				isJumping = true;
				_movement.Jump();
			}
		}
	}
	
	private void CheckAttack()
	{				
		if(! canAttack)
			return;

		if(Time.time < _lastAttack + AttackLockout)
			return;

		if(! Input.GetButtonDown("Fire1"))
		{
			isAttacking = false;
			return;
		}

		DebugMessage("The player is attacking!");

		_lastAttack = Time.time;
		isAttacking = true;

		if(attackSound != null)
			_maestro.PlaySoundEffect(attackSound);
	}

	private void CheckCrouching()
	{	
		if(_movement.MovementType != SidescrollingMovementType.Grounded
		   || Input.GetAxis("Vertical") >= 0)
		{
			isCrouching = false;
			return;
		}
		
		DebugMessage("The player is crouching!");
        isCrouching = true;
        isMovingHorizontally = false;
		_movement.HaltMotion();
    }
	
	private void CheckHorizontal()
	{
		if(! Input.GetButton("Horizontal"))
		{
			isMovingHorizontally = false;
			_movement.ClearHorizontalMovement();
			return;
		}

		isFacingRight = Input.GetAxis("Horizontal") > 0;

		if(isCrouching)
			return;

		DebugMessage("The player is moving!");
		isMovingHorizontally = true;

		_movement.MoveHorizontally(isFacingRight);
	}
	
	private void CheckOverthrust()
	{	
		if(! canOverthrust
		   || _movement.MovementType == SidescrollingMovementType.Grounded
		   || _movement.MovementType == SidescrollingMovementType.Hit
		   || Input.GetAxisRaw("Vertical") <= 0)
		{
			isOverthrusting = false;
			return;
		}
				
		DebugMessage("Performing Overthrust!");
		isOverthrusting = true;
	}
	
	private void CheckUnderthrust()
	{
		if(! canUnderthrust
		   || _movement.MovementType == SidescrollingMovementType.Grounded
		   || _movement.MovementType == SidescrollingMovementType.Hit
		   || Input.GetAxisRaw("Vertical") >= 0)
		{
			isUnderthrusting = false;
			return;
		}

		DebugMessage("Performing Underthrust!");
		isUnderthrusting = true;
	}
	
	private void CheckInjured()
	{
		if(_movement.MovementType != SidescrollingMovementType.Hit)
			return;

		DebugMessage("The player has been hit!");
		isHit = true;
	}

	private void DetermineControlState()
	{
		controlState = isFacingRight ? PlayerControlState.IdleRight : PlayerControlState.IdleLeft;

		if(isHit)
		{
			controlState = isFacingRight ? PlayerControlState.HitRight : PlayerControlState.HitLeft;
			return;
		}

		if(isUnderthrusting)
		{
			controlState = PlayerControlState.Underthrust;
			return;
		}

		if(isOverthrusting)
		{
			controlState = PlayerControlState.Overthrust;
			return;
		}

		if(isAttacking)
		{
			// If crouching, use crouch attack animation.
			if(isCrouching)
				controlState = isFacingRight ? PlayerControlState.CrouchAttackRight : PlayerControlState.CrouchAttackLeft;
			// Jumping/Falling attacks use the crouch attack animation.
			else if(_movement.MovementType != SidescrollingMovementType.Grounded && _movement.MovementType != SidescrollingMovementType.Hit)
				controlState = isFacingRight ? PlayerControlState.CrouchAttackRight : PlayerControlState.CrouchAttackLeft;
			// Otherwise use standing animation.
			else
				controlState = isFacingRight ? PlayerControlState.AttackRight : PlayerControlState.AttackLeft;
            
            return;
        }

		if(isCrouching)
		{
			controlState = isFacingRight ? PlayerControlState.CrouchRight : PlayerControlState.CrouchLeft;
			return;
		}

		if(isJumping)
		{
			controlState = isFacingRight ? PlayerControlState.JumpRight : PlayerControlState.JumpLeft;
			return;
		}

		if(isMovingHorizontally)
		{
			switch(_movement.MovementType)
			{
				case SidescrollingMovementType.Grounded:
					controlState = isFacingRight ? PlayerControlState.WalkRight : PlayerControlState.WalkLeft;
					break;
					
				case SidescrollingMovementType.Falling:
					controlState = isFacingRight ? PlayerControlState.FallRight : PlayerControlState.FallLeft;
					break;
			}

			return;
		}
	}
	
	#endregion Methods

	#region Implementation of IPausableEntity
	
	public void PauseThisEntity()
	{
		if(DebugMode)
			Debug.Log("Pausing player controls!");
		
		allowAction = false;
		_movement.HaltMotion();
		_movement.HaltJump();
	}
	
	public void ResumeThisEntity()
	{
		if(DebugMode)
			Debug.Log("Resuming player controls!");
		
		allowAction = true;
	}
	
	#endregion Implementation of IPausableEntity
}
