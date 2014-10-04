using System;
using UnityEngine;
using System.Collections;

public class Maestro : MonoBehaviour 
{
	#region Variables / Properties
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Awake()
	{
	}
	
	void Update()
	{
		AudioListener.volume = Settings.masterVolume;
		audio.volume = Settings.musVolume;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public static Maestro DetectLastInstance()
	{
		return (Maestro) FindObjectOfType(typeof(Maestro));
	}
	
	public void PlaySoundEffect(AudioClip effect, float fxVolume = 0.0f)
	{
		// A given FX volume overrides the SFX volume specified in the settings.
		if(fxVolume == 0.0f)
			fxVolume = Settings.sfxVolume;

		audio.PlayOneShot(effect, fxVolume);
	}
	
	public void ChangeTunes(AudioClip newChart)
	{	
		if(newChart == null) 
			throw new ArgumentNullException("newChart");

		float currentTime = audio.time;

		audio.clip = newChart;
		audio.time = currentTime;
		audio.Play();
	}
	
	public void NewTune(AudioClip newChart)
	{
		if(newChart == null)
			throw new ArgumentNullException("newChart");
		
		audio.clip = newChart;
		audio.time = 0.0f;
		audio.Play();
	}
	
	#endregion Methods
}
