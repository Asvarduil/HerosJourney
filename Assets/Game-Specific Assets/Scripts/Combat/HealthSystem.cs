using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour 
{
	#region Constants

	private const string _changeEvent = "OnHealthChanged";

	#endregion Constants

	#region Variables / Properties
	
	public bool DebugMode = false;
	public bool CanSpawnEffects = true;

	public int HP;
	public int MaxHP;
	public GameObject DamageEffect;
	public GameObject DeathEffect;
	
	private Shield _shield;
	private Ambassador _ambassador;
	private PlayerHealthProvider _healthProvider;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_shield = GetComponentInChildren<Shield>();
		_ambassador = Ambassador.Instance;
		_healthProvider = (PlayerHealthProvider) FindObjectOfType(typeof(PlayerHealthProvider));
	}

	public void OnDestroy()
	{
		CanSpawnEffects = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public bool ShieldTookImpact()
	{
		if(_shield == null)
			return false;
		
		if(DebugMode)
			Debug.Log("Attached shield " + (_shield.AbsorbedImpact ? "absorbed" : "did not block") + " the blow.");
		
		return _shield.AbsorbedImpact;
	}

	public void IncreaseCapacity(int amount)
	{
		if(! enabled)
			return;

		HP += amount;
		MaxHP += amount;
		_ambassador.MaxHP += amount;

		NotifyOtherObjects();
	}
	
	public void TakeDamage(int damage)
	{
		if(! enabled)
			return;
		
		if(ShieldTookImpact())
			return;

		ShowDamageEffect();
		HP -= (damage > HP)
			? HP
			: damage;

		NotifyOtherObjects();
		
		if(DebugMode)
			Debug.Log(String.Format("{0} took {1} damage.\r\n{0} has {2} HP left.", gameObject.name, damage, HP));
		
		CheckForDeath();
	}
	
	public void Heal(int amount)
	{
		if(!enabled)
			return;
		
		if(DebugMode)
			Debug.Log(gameObject.name + " was healed for " + amount);
		
		HP += (HP + amount > MaxHP)
			? (MaxHP - HP)
			: amount;

		NotifyOtherObjects();
	}

	private void ShowDamageEffect()
	{
		if(! CanSpawnEffects)
			return;

		GameObject.Instantiate(DamageEffect, transform.position, transform.rotation);
	}

	private void ShowDeathEffect()
	{
		if(! CanSpawnEffects)
			return;

		GameObject.Instantiate(DeathEffect, transform.position, transform.rotation);
	}

	private void CheckForDeath()
	{
		if(HP > 0)
			return;

		if(DebugMode)
			Debug.Log(gameObject.name + " died.");
		
		ShowDeathEffect();
		Destroy(gameObject);
	}

	private void NotifyOtherObjects()
	{
		if(DebugMode)
			Debug.Log("Notifying other objects about an HP change!");

		HealthEventArgs args = new HealthEventArgs(HP, MaxHP, gameObject.tag);
		_ambassador.SendMessage(_changeEvent, args, SendMessageOptions.DontRequireReceiver);
		_healthProvider.SendMessage(_changeEvent, args, SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion Methods
}

public class HealthEventArgs
{
	public int HP;
	public int MaxHP;
	public string Tag;

	public HealthEventArgs(int hp, int maxhp, string tag)
	{
		HP = hp;
		MaxHP = maxhp;
		Tag = tag;
	}
}
