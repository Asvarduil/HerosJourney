using UnityEngine;
using System.Collections.Generic;

public class Shield : MonoBehaviour {
	#region Variables / Properties
	
	public bool DebugMode = false;
	public bool AbsorbedImpact = false;
	
	public AudioClip AbsorbSound;
	public List<string> AbsorbTags = new List<string> { "Projectile" };
	public List<string> DeflectTags = new List<string> { "Sword" };
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	// Update is called once per frame
	public void OnTriggerEnter(Collider collider)
	{
		if(AbsorbTags.Contains(collider.tag))
		{
			if(DebugMode)
				Debug.Log("Absorbed impact from " + collider.gameObject.name);
			
			GameObject.Destroy(collider.gameObject);
			
			AbsorbedImpact = true;
			if(AbsorbSound != null)
				audio.PlayOneShot(AbsorbSound);
		}
		
		if(DeflectTags.Contains(collider.tag))
		{
			if(DebugMode)
				Debug.Log("Deflected impact from " + collider.gameObject.name);
			
			AbsorbedImpact = true;
			if(AbsorbSound != null)
				audio.PlayOneShot(AbsorbSound);
		}
	}
	
	public void LateUpdate()
	{
		AbsorbedImpact = false;
	}
	
	#endregion Engine Hooks
}
