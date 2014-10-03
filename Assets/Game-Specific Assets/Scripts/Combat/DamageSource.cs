using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class DamageSource : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public AudioClip DamageSound;
	public int AttackPower = 1;
	public float RepelForce = 26.5f;
	public List<string> AffectedTags = new List<string>{"Enemy"};
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		bool willBeAffected = AffectedTags.Contains(who.tag);
		if(! willBeAffected)
			return;
		
		if(DamageSound != null)
			_maestro.PlaySoundEffect(DamageSound);
		
		RepelDamagedEntity(who.gameObject);
		who.SendMessage("TakeDamage", AttackPower, SendMessageOptions.DontRequireReceiver);
		who.SendMessage("RepelAttacker", gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void RepelDamagedEntity(GameObject target)
	{
		GameObject root = target;
		if(root == null)
		{
			return;
		}
		
		SidescrollingMovement move = root.GetComponent<SidescrollingMovement>();
		if(move == null)
		{
			return;
		}
		
		move.RepelFromObject(gameObject, RepelForce);
	}
	
	#endregion Methods
}
