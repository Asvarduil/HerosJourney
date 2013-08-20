using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class BossAI : BaseAI
{
	#region Variables / Properties
	
	public bool FightStarted = false;
	
	protected string _currentAnimation;
	
	protected PlayerSense _sense;
	protected SpriteSystem _sprite;
	protected HitboxController _hitboxes;
	protected SidescrollingMovement _movement;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Update()
	{
		base.Update();
		PlayAnimations();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
		_hitboxes.PlaySingleFrame(_currentAnimation, false, AnimationMode.Loop);
	}
	
	#endregion Methods
}
