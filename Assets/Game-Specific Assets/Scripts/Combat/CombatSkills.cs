using UnityEngine;
using System;
using System.Collections;

// Programmer's Notes: 
// ----------------------------------------------
// Combat skills are somewhat specific in this
// case.  Because this system draws on the
// Zelda 2 conventions, there are two main
// abilities: Overthrust and Underthrust.
//
// This script communicates with the Ambassador
// to determine whether the resident player piece
// should have access to these abilities.  This
// script also goes further and sets up the
// health system of the player on scene load
// as well.

public class CombatSkills : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public string OverthrustSkill;
	public string UnderthrustSkill;
	
	private Ambassador _ambassador;
	private PlayerControl _control;
	private HealthSystem _health;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		_control = GetComponent<PlayerControl>();
		_health = GetComponent<HealthSystem>();
		
		if(_ambassador == null
		   || _control == null
		   || _health == null)
		{
			if(DebugMode)
				Debug.LogWarning("Was unable to find player controls or ambassador!" + Environment.NewLine
					             + "Ambassador: " + ((_ambassador == null) ? "Not Found" : "Found") + Environment.NewLine
					             + "Player Controls: " + ((_control == null) ? "Not Found" : "Found") + Environment.NewLine
					             + "Health System: " + ((_health == null) ? "Not Found" : "Found"));
			
			return;
		}
		
		_control.canOverthrust = _ambassador.HasItem(OverthrustSkill);
		_control.canUnderthrust = _ambassador.HasItem(UnderthrustSkill);
		_health.HP = _ambassador.MaxHP;
		_health.MaxHP = _ambassador.MaxHP;
		
		if(DebugMode)
			Debug.Log("Player " + (_control.canOverthrust ? "can" : "cannot") + " perform Overthrust." + Environment.NewLine
				      + "Player " + (_control.canUnderthrust ? "can" : "cannot") + " perform Underthrust."
				      + "Player's HP: " + _health.HP + "/" + _health.MaxHP);
	}
	
	#endregion Engine Hooks
}
