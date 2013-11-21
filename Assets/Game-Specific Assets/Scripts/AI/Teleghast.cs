using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Teleghast : AIBase
{
	#region Variables / Properties
	
	public float ConjureTime = 1.0f;
	public float WaitTime = 2.0f;
	public float TeleportTime = 2.0f;
	
	public List<Vector3> AnchorPoints;
	
	public string IdleLeft;
	public string IdleRight;
	public string CastHighLeft;
	public string CastLowLeft;
	public string CastHighRight;
	public string CastLowRight;
	
	public int LaunchHighRate = 50;
	public GameObject Muzzle;
	public GameObject ConjureEffect;
	public GameObject Projectile;
	public GameObject TeleportEffect;
	
	private int _currentAction = 0;
	private bool _facingLeft = true;
	private float _nextAction;
	private string _animation;
	
	private GameObject _conjureEffect;
	
	private HealthSystem _health;
	private HitboxController _boxController;
	private List<Action> _states;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Start()
	{
		base.Start();
		_health = GetComponent<HealthSystem>();
		_boxController = GetComponentInChildren<HitboxController>();
		
		_states = new List<Action> {
			AppearAtAnAnchor,
			ConjureProjectile,
			LaunchProjectile,
			WatchTheBolt,
			Disappear
		};
		
		_facingLeft = true;
		_animation = IdleLeft;
	}
	
	public void Update()
	{
		if(_isPaused)
			return;
		
		PlayAnimations();
		
		if(Time.time < _nextAction)
			return;
		
		// Get the current behavior and run it.
		Action behavior = _states[_currentAction];
		behavior();
		
		// Increment state; if beyond end, go back to first state.
		_currentAction = (_currentAction + 1) % _states.Count;
	}
	
	public void OnDestroy()
	{
		CleanupConjureEffect();
		enabled = false;
	}
	
	#endregion Engine Hooks
	
	#region Behaviors
	
	public void AppearAtAnAnchor()
	{
		PresentMe(true);
		
		int index = Random.Range(0, AnchorPoints.Count);
		Vector3 anchor = AnchorPoints[index];
		transform.position = anchor;
		
		if(_sense.DetectedPlayer)
		{
			float playerX = _sense.PlayerLocation.position.x;
			_facingLeft = playerX < transform.position.x;
		}
		
		_animation = _facingLeft ? IdleLeft : IdleRight;
	}
	
	public void ConjureProjectile()
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		// Perform the appropriate animation.
		int highLowRoll = Random.Range(0, 100);
		_animation = (highLowRoll < LaunchHighRate)
						? _facingLeft
							? CastHighLeft
							: CastHighRight
						: _facingLeft
							? CastLowLeft
							: CastLowRight;
		
		CleanupConjureEffect();
		_conjureEffect = (GameObject) GameObject.Instantiate(ConjureEffect, muzzlePoint, Quaternion.identity);
		
		SetupNextActionTime(ConjureTime);
	}
	
	public void LaunchProjectile()
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		CleanupConjureEffect();
		GameObject bolt = (GameObject) GameObject.Instantiate(Projectile, muzzlePoint, Quaternion.identity);
		Projectile guidance = bolt.GetComponent<Projectile>();
		
		float absoluteVelocity = guidance.Velocity.x;
		float newVelocity = _facingLeft ? absoluteVelocity : -absoluteVelocity;
		
		guidance.Velocity.x = newVelocity;
		guidance.enabled = true;
	}
	
	public void WatchTheBolt()
	{
		SetupNextActionTime(WaitTime);
	}
	
	public void Disappear()
	{
		Instantiate(TeleportEffect, transform.position, Quaternion.identity);
		
		PresentMe(false);
		SetupNextActionTime(TeleportTime);
	}
	
	#endregion Behaviors
	
	#region Methods
	
	public override void PlayAnimations()
	{
		_sprite.PlaySingleFrame(_animation, true, AnimationMode.Loop);
		_boxController.PlaySingleFrame(_animation, true, AnimationMode.Loop);
	}
	
	private void SetupNextActionTime(float timeOffset)
	{
		_nextAction = Time.time + timeOffset;
		
		if(DebugMode)
			Debug.Log("Next action at: " + _nextAction);
	}
	
	private void CleanupConjureEffect()
	{
		if(_conjureEffect == null)
			return;
		
		if(DebugMode)
			Debug.Log("Cleaning up an un-deleted conjure effect.");
		
		Destroy(_conjureEffect);
		_conjureEffect = null;
	}
	
	private void PresentMe(bool isPresented)
	{
		if(DebugMode)
			Debug.Log((isPresented ? "Showing" : "Hiding") + " " + gameObject.name);
		
		_health.enabled = isPresented;
		_sprite.gameObject.SetActive(isPresented);
	}
	
	#endregion Methods
}
