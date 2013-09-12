using UnityEngine;
using System;
using System.Collections.Generic;

public class BossRoomTrigger : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool debugMode = false;
	public bool selfDestructOnActivate = false;
	public List<string> affectedTags = new List<string>{"Player"};
	public GameObject activateObjectGroup;
	public AudioClip activateObjectSFX;
	public AudioClip bgmOverride;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(! affectedTags.Contains(who.tag))
			return;
		
		if(_maestro == null)
			throw new Exception("No Maestro was detected onload...");
		
		if(activateObjectSFX != null)
			_maestro.PlaySoundEffect(activateObjectSFX);
		
		if(activateObjectGroup != null)
			activateObjectGroup.SetActive(true);
		
		if(bgmOverride != null)
			_maestro.NewTune(bgmOverride);
		
		if(selfDestructOnActivate)
			Destroy(gameObject);
	}
	
	#endregion Engine Hooks
}
