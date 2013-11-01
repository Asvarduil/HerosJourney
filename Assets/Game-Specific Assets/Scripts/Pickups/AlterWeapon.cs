using UnityEngine;
using System.Collections;

public class AlterWeapon : MonoBehaviour 
{
	#region Variables / Properties
	
	public string AffectedTag = "Player";
	public string ConferWeaponTag = "Breakable";
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		DamageSource weapon = who.gameObject.GetComponentInChildren<DamageSource>();
		weapon.AffectedTags.Add(ConferWeaponTag);
	}
	
	#endregion Engine Hooks
}
