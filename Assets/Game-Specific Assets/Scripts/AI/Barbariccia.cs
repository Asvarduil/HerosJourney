using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Barbariccia : AIBase, IPausableEntity
{	
	#region Variables / Properties

	public float ProjectileSpeed = 5.0f;
	public float ConjureTime = 1.0f;
	public float WaitTime = 2.0f;
	public float TeleportTime = 2.0f;
	
	public string IdleLeft = "Idle-Left";
	public string IdleRight = "Idle-Right";
	public string CastLeft = "Cast-Left";
	public string CastRight = "Cast-Right";
	public string CastTop = "Cast-Top";
	
	public List<Vector3> AnchorPoints;
	
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
		
		_states = new List<Action>{
			AppearAtAnAnchor,
			ConjureProjectile,
			LaunchProjectileAtPlayer,
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
	}
	
	#endregion Engine Hooks
	
	#region Behaviors
	
	public void AppearAtAnAnchor()
	{
		PresentMe(true);
		
		int index = Random.Range(0, AnchorPoints.Count);
		Vector3 anchor = AnchorPoints[index];
		transform.position = anchor;

		try
		{
			if(_sense.DetectedPlayer)
			{
				float playerX = _sense.PlayerLocation.position.x;
				_facingLeft = playerX < transform.position.x;
			}
		}
		catch
		{
			if(DebugMode)
				Debug.LogWarning("The player no longer exists.  Hibernating this game object.");

			gameObject.SetActive(false);
		}
		
		_animation = _facingLeft ? IdleLeft : IdleRight;
	}

	public void ConjureProjectile() 
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		CleanupConjureEffect();
		_conjureEffect = (GameObject) GameObject.Instantiate(ConjureEffect, muzzlePoint, Quaternion.identity);
		
		SetupNextActionTime(ConjureTime);
	}
	
	public void LaunchProjectileAtPlayer()
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		CleanupConjureEffect();
		GameObject bolt = (GameObject) GameObject.Instantiate(Projectile, muzzlePoint, Quaternion.identity);
		Projectile guidance = bolt.GetComponent<Projectile>();
		
		float absoluteVelocity = guidance.Velocity.x;
		Vector3 newVelocity = Vector3.Normalize(_sense.PlayerLocation.position - transform.position) * ProjectileSpeed;

		if(DebugMode)
			Debug.Log("Firing Big Blue Magic bolt at a velocity: " + newVelocity.ToString());
		
		guidance.Velocity = newVelocity;
		guidance.enabled = true;
	}
	
	public void WatchTheBolt()
	{
		SetupNextActionTime(WaitTime);
	}
	
	public void Disappear()
	{
		GameObject.Instantiate(TeleportEffect, transform.position, Quaternion.identity);
		
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
