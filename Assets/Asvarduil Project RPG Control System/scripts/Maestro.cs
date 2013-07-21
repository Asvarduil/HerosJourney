using System;
using UnityEngine;
using System.Collections;

public class Maestro : MonoBehaviour 
{
	#region Variables / Properties
	
	public float fadeRate = 0.9f;
	public float threshold = 0.05f;
	
	private float maxVolume;
	private float targetVolume;
	
	private bool _IsFadingOut = false;
	private bool _IsFadingIn = false;
	public bool IsFading
	{
		get { return _IsFadingOut || _IsFadingIn; }
	}
	
	public bool IsSilent
	{
		get { return Mathf.Abs(audio.volume - targetVolume) <= threshold; }
	}
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start()
	{
		FadeIn();
	}
	
	void FixedUpdate()
	{
		if(IsFading)
		{			
			audio.volume = Mathf.Lerp(audio.volume, targetVolume, fadeRate * Time.deltaTime);
			
			if(audio.volume <= threshold)
			{
				audio.volume = targetVolume;
				_IsFadingOut = false;
			}
			
			if (audio.volume >= 1.0f - threshold)
			{
				audio.volume = targetVolume;
				_IsFadingIn = false;
			}
		}
		else
		{
			audio.volume = Settings.musVolume;
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public static Maestro DetectLastInstance()
	{
		return (Maestro) FindObjectOfType(typeof(Maestro));
	}
	
	public void PlaySoundEffect(AudioClip effect)
	{
		audio.PlayOneShot(effect, Settings.sfxVolume);
	}
	
	public void ChangeTunes(AudioClip newChart)
	{	
		if(newChart == null) throw new ArgumentNullException("newChart");

		float currentTime = audio.time;

		audio.clip = newChart;
		audio.time = currentTime;
		audio.Play();
	}
	
	public void FadeIn()
	{
		audio.volume = 0.0f;
		targetVolume = Settings.musVolume;
		_IsFadingIn = true;
	}
	
	public void FadeOut()
	{
		audio.volume = Settings.musVolume;
		targetVolume = 0.0f;
		_IsFadingOut = true;
	}
	
	#endregion Methods
}
