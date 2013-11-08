using UnityEngine;
using System;
using System.Collections.Generic;

public class ParriesBlueMagic : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public AudioClip DeflectSound;
	public string ItemName = "Hero's Hilt";
	public string DeflectedTarget = "Enemy";
	public List<string> DefendableItemTags = new List<string>{"Blue Magic"};
	
	public bool DeflectsBlueMagic 
	{ 
		get { return _ambassador.HasItem(ItemName); } 
	}
	
	private Ambassador _ambassador;
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		_maestro = Maestro.DetectLastInstance();
		
		if(_ambassador == null)
		{
			if(DebugMode)
				Debug.LogWarning("Was unable to find damage source or ambassador!" + Environment.NewLine
					             + "Ambassador: " + (_ambassador == null ? "Not Found" : "Found"));
			
			return;
		}
		
		if(DebugMode)
			Debug.Log("This weapon " + (DeflectsBlueMagic ? "can" : "can't") + " repel Blue Magic.");	
	}
	
	public void OnTriggerEnter(Collider who)
	{	
		bool isDeflectableObject = DefendableItemTags.Contains(who.tag);
		
		if(! DeflectsBlueMagic
		   || ! isDeflectableObject)
		{
			if(DebugMode)
			{
				Debug.Log("This weapon " + (DeflectsBlueMagic ? "can" : "cannot") + "deflect Blue Magic.");
				Debug.Log("GameObject " + who.gameObject.name + " " + (isDeflectableObject ? "is" : "is not") + " a deflectable object.");
			}
			
			return;
		}
		
		if(DeflectSound != null)
			_maestro.PlaySoundEffect(DeflectSound);
		
		GameObject original = who.gameObject;
		Projectile originalProjectile = original.GetComponent<Projectile>();
		DamageSource originalDamageSource = original.GetComponent<DamageSource>();
		
		originalProjectile.Velocity *= -1;
		originalProjectile.AffectedTags = new List<string>{ DeflectedTarget };
		
		originalDamageSource.AffectedTags = new List<string>{ DeflectedTarget };
	}
	
	#endregion Engine Hooks
}
