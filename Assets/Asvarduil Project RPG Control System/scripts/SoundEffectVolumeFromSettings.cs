using UnityEngine;
using System.Collections;

public class MusicVolumeFromSettings : MonoBehaviour 
{
	#region Variables / Properties
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Awake()
	{
		audio.volume = Settings.sfxVolume;
	}
	
	public void Update()
	{
		audio.volume = Settings.sfxVolume;
	}
	
	#endregion Engine Hooks
}
