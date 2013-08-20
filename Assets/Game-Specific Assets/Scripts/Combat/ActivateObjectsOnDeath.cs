using UnityEngine;
using System.Collections.Generic;

public class ActivateObjectsOnDeath : MonoBehaviour 
{
	#region Variables / Properties
	
	public List<GameObject> Objects;
	public GameObject SpecialEffect;
	public AudioClip SoundEffect;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	public void OnDestroy()
	{
		if(Objects != null
		   && Objects.Count > 0)
			foreach(GameObject current in Objects)
				if(current != null)
					current.SetActive(true);
		
		if(SpecialEffect != null)
			Instantiate(SpecialEffect, transform.position, transform.rotation);
		
		if(SoundEffect != null
		   && _maestro != null)
			_maestro.PlaySoundEffect(SoundEffect);
	}
	
	#endregion Engine Hooks
}
