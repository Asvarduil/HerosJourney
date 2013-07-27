using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
	#region Variables / Properties
	
	public string AffectedTag = "Player";
	public int HealAmount = 2;
	public AudioClip HealingSound;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		if(HealingSound != null)
			_maestro.PlaySoundEffect(HealingSound);
		
		who.gameObject.SendMessage("Heal", HealAmount, SendMessageOptions.DontRequireReceiver);
		Destroy(gameObject);
	}
	
	#endregion Engine Hooks
}
