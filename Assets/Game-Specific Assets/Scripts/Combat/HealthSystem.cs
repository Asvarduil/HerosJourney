using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public int HP;
	public int MaxHP;
	public GameObject DeathEffect;
	
	private Shield _shield;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_shield = GetComponentInChildren<Shield>();
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
	
	public void TakeDamage(int damage)
	{
		if(ShieldTookImpact())
			return;
		
		HP -= (damage > HP)
			? HP
			: damage;
		
		if(DebugMode)
			Debug.Log(String.Format("{0} took {1} damage.\r\n{0} has {2} HP left.", gameObject.name, damage, HP));
		
		if(HP == 0)
		{
			if(DebugMode)
				Debug.Log(gameObject.name + " died.");
			
			GameObject.Instantiate(DeathEffect, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
	
	public void Heal(int amount)
	{
		if(DebugMode)
			Debug.Log(gameObject.name + " was healed for " + amount);
		
		HP += (HP + amount > MaxHP)
			? (MaxHP - HP)
			: amount;
	}
	
	#endregion Methods
}
