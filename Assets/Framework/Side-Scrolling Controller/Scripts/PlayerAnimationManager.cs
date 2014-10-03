using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimationManager : DebuggableBehavior
{
	#region Variables / Properties
	
	public List<PlayerAnimationKey> animations = new List<PlayerAnimationKey>();
	
	private AnimationMode _animationMode;

	private PlayerControl _control;
	private SpriteSystem _animation;
	private HitboxController _hitboxController;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Start()
	{
		_control = GetComponent<PlayerControl>();
		_animation = GetComponentInChildren<SpriteSystem>();
		_hitboxController = gameObject.GetComponentInChildren<HitboxController>();
	}

	#endregion Engine Hooks

	#region Methods

	public void PerformAnimation()
	{
		string animation = DetermineAnimationSequence();

		PerformHitboxAnimation(animation);
		PerformSpriteAnimation(animation);
	}

	private string DetermineAnimationSequence()
	{
		return animations.FirstOrDefault(a => a.state == _control.controlState).animation;
	}

	private void PerformHitboxAnimation(string animation)
	{
		_hitboxController.PlaySingleFrame(animation, true, _animationMode);
	}
	
	private void PerformSpriteAnimation(string animation)
	{
		_animation.PlaySingleFrame(animation, true, _animationMode);
	}

	#endregion Methods
}
