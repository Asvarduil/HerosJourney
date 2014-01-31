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
	
	private Teleportation _teleportation;
	private HitboxController _boxController;
	private SpiralProjectileSpam _spiralProjectileSpam;

	private List<Action> _states;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public override void Start()
	{
		base.Start();

		_teleportation = GetComponent<Teleportation>();
		_boxController = GetComponentInChildren<HitboxController>();
		_spiralProjectileSpam = GetComponent<SpiralProjectileSpam>();
		
		_states = new List<Action>{
			TeleportToAnchor,
			ConjureProjectile,
			LaunchProjectileAtPlayer,
			WatchTheBolt,
			TeleportToAnchor,
			ConjureProjectile,
			SpamMagic,
			WatchTheBolt,
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
	}
	
	public void OnDestroy()
	{
		CleanupConjureEffect();
	}
	
	#endregion Engine Hooks
	
	#region Behaviors
	
	public void TeleportToAnchor()
	{
		int index = Random.Range(0, AnchorPoints.Count);
		Vector3 anchor = AnchorPoints[index];

		anchor = _teleportation.ConstraintPointToRectangle(anchor);
		_teleportation.TeleportToPoint(anchor);

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
		AdvanceState();
	}

	public void ConjureProjectile() 
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		CleanupConjureEffect();
		_conjureEffect = (GameObject) GameObject.Instantiate(ConjureEffect, muzzlePoint, Quaternion.identity);
		
		SetupNextActionTime(ConjureTime);
		_spiralProjectileSpam.ResetSpam();
	}
	
	public void LaunchProjectileAtPlayer()
	{
		Vector3 muzzlePoint = Muzzle.transform.position;
		
		CleanupConjureEffect();
		GameObject bolt = (GameObject) GameObject.Instantiate(Projectile, muzzlePoint, Quaternion.identity);
		Projectile guidance = bolt.GetComponent<Projectile>();

		try
		{
			Vector3 newVelocity = Vector3.Normalize(_sense.PlayerLocation.position - transform.position) * ProjectileSpeed;

			if(DebugMode)
				Debug.Log("Firing Big Blue Magic bolt at a velocity: " + newVelocity.ToString());
		
			guidance.Velocity = newVelocity;
			guidance.enabled = true;
		}
		catch(Exception ex)
		{
			if(DebugMode)
				Debug.LogException(ex);
		}

		AdvanceState();
	}

	public void SpamMagic()
	{
		_spiralProjectileSpam.FireSomeSpam();

		if(_spiralProjectileSpam.IsDoneSpamming)
			AdvanceState();
	}
	
	public void WatchTheBolt()
	{
		SetupNextActionTime(WaitTime);
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
		AdvanceState();
		
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

	private void AdvanceState()
	{
		// Increment state; if beyond end, go back to first state.
		_currentAction = (_currentAction + 1) % _states.Count;
	}

	#endregion Methods
}
