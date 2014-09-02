using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SettingsGUI : MonoBehaviour
{
	#region Variables / Properties

	public bool shown = false;
	
	public GUISkin skin;
	public AudioClip ButtonSound;
	
	public OldSettingsForm SettingsForm;
	
	private Maestro _Maestro;
	public Maestro MyMaestro
	{
		get
		{
			return _Maestro
				   ?? (_Maestro = Maestro.DetectLastInstance());
		}
	}
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void OnGUI()
	{
		if(! shown)
			return;
		
		GUI.skin = skin;
		
		SettingsForm.DrawMe();
		
		SettingsForm.CheckSoundSettings();
		SettingsForm.CheckControlSettings();
		SettingsForm.CheckGraphicsSettings();
		
		CheckIfClosed();
	}
	
	public void FixedUpdate()
	{
		SettingsForm.Tween();
	}
	
	public void Show()
	{
		shown = true;
		SettingsForm.Show();
	}
	
	public bool CheckIfClosed()
	{
		if(SettingsForm.IsClosed())
		{
			MyMaestro.PlaySoundEffect(ButtonSound);
			
			SettingsForm.Hide();
			shown = false;
			return true;
		}
		
		return false;
	}
	
	#endregion Methods
}


