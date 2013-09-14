using UnityEngine;
using System;
using System.Collections.Generic;

public class ParriesBlueMagic : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public float CloneLockout = 0.5f;
	public string ItemName = "Hero's Hilt";
	public string DeflectedTarget = "Enemy";
	public List<string> DefendableItemTags = new List<string>{"Blue Magic"};
	
	private float _lastClone;
	private DamageSource _damageSource;
	private Ambassador _ambassador;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		_damageSource = GetComponent<DamageSource>();
		
		if(_ambassador == null
		   || _damageSource == null)
		{
			if(DebugMode)
				Debug.LogWarning("Was unable to find damage source or ambassador!" + Environment.NewLine
					             + "Ambassador: " + ((_ambassador == null) ? "Not Found" : "Found") + Environment.NewLine
					             + "Damage Source: " + ((_damageSource == null) ? "Not Found" : "Found"));
			
			return;
		}
		
		if(_ambassador.HasItem(ItemName))
		{
			if(DebugMode)
				Debug.Log("This damage trigger can repel Blue Magic.");
			
			_damageSource.AffectedTags.AddRange(DefendableItemTags);
		}
		else
		{
			if(DebugMode)
				Debug.Log("This damage trigger can't repel Blue Magic.");
		}
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(Time.time < _lastClone + CloneLockout)
			return;
		
		if(! DefendableItemTags.Contains(who.tag))
			return;
		
		// Clone the Blue Magic bolt, reverse its trajectory, and who it affects.
		GameObject original = who.gameObject;
		Projectile originalProjectile = original.GetComponent<Projectile>();
		
		GameObject bolt = (GameObject) Instantiate(original, original.transform.position, original.transform.rotation);
		
		Projectile boltProjectile = bolt.GetComponent<Projectile>();
		boltProjectile.AffectedTags = new List<string>{ DeflectedTarget };
		boltProjectile.Velocity = originalProjectile.Velocity * -1;
		boltProjectile.enabled = true;
		
		DamageSource boltDamage = bolt.GetComponent<DamageSource>();
		boltDamage.AffectedTags = new List<string>{ DeflectedTarget };
		boltDamage.enabled = true;
		
		_lastClone = Time.time;
		Destroy(original);
	}
	
	#endregion Engine Hooks
}
