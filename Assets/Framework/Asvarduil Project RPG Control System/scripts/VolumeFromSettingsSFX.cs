using UnityEngine;
using System.Collections;

public class VolumeFromSettingsSFX : MonoBehaviour 
{
	public void Awake() 
	{
		audio.volume = Settings.sfxVolume;
	}

	public void Update() 
	{
		audio.volume = Settings.sfxVolume;
	}
}
