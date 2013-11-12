using UnityEngine;
using System.Collections;

public class SoundEffectVolumeFromSettings : MonoBehaviour 
{
	#region Variables / Properties
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Awake()
	{
		audio.volume = Settings.musVolume;
	}
	
	public void Update()
	{
		audio.volume = Settings.musVolume;
	}
	
	#endregion Engine Hooks
}
