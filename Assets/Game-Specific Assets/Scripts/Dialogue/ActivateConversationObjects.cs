using UnityEngine;
using System.Collections.Generic;

public class ActivateConversationObjects : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public AudioClip SoundEffect;
	public List<GameObject> Objects;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	#endregion Engine Hooks
	
	#region Methods
  
	public void ActivateGameObjects()
	{
		if(DebugMode)
			Debug.Log("Activating all linked game objects...");
		
		if(SoundEffect != null)
			_maestro.PlaySoundEffect(SoundEffect);
		
		foreach(GameObject current in Objects)
		    current.SetActive(true);
	}
	
	public void DeactivateGameObjects()
	{
		if(DebugMode)
			Debug.Log("Deactivating all linked game objects...");
		
		if(SoundEffect != null)
			_maestro.PlaySoundEffect(SoundEffect);
		
		foreach(GameObject current in Objects)
		    current.SetActive(false);
	}
	
	#endregion Methods
}
