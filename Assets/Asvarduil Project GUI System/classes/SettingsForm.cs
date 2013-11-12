using UnityEngine;
using System;

[Serializable]
public class OldSettingsForm : AsvarduilForm
{
	#region Variables / Properties
	
	public AsvarduilCheckbox Mute;
	public AsvarduilCheckbox UseWasd;
	public AsvarduilCheckbox UseMouse;
	
	public AsvarduilSlider MasterVolume;
	public AsvarduilSlider MusicVolume;
	public AsvarduilSlider EffectsVolume;
	
	public AsvarduilSlider GraphicsQuality;
	
	public AsvarduilButton Close;
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public OldSettingsForm(AsvarduilImage bg, AsvarduilLabel winName)
		: base(bg, winName)
	{
	}
	
	#endregion Constructor
	
	#region Methods
	
	public void LoadSettings()
	{
		Mute.Value = Settings.soundEnabled;
		MasterVolume.Value = Settings.masterVolume;
		MusicVolume.Value = Settings.musVolume;
		EffectsVolume.Value = Settings.sfxVolume;
		GraphicsQuality.Value = Settings.graphicsLevel;
		UseWasd.Value = Settings.useFourAxisControl;
		UseMouse.Value = Settings.useMouseControl;
	}
	
	public void CheckSoundSettings()
	{
		if(Mute.IsClicked())
		{
			if(Mute.Value)
			{
				Mute.Text = "Sound Enabled";
			}
			else
			{
				Mute.Text = "Sound Disabled";
			}
			
			Settings.masterVolume = MasterVolume.IsMoved();
			Settings.musVolume = MusicVolume.IsMoved();
			Settings.sfxVolume = EffectsVolume.IsMoved();
			
			Settings.soundEnabled = Mute.Value;
			AudioListener.volume = Settings.soundEnabled
				? Settings.masterVolume
				: 0;
		}
	}
	
	public void CheckControlSettings()
	{
		Settings.useFourAxisControl = UseWasd.IsClicked();
		Settings.useMouseControl = UseMouse.IsClicked();
	}
	
	public void CheckGraphicsSettings()
	{
		Settings.graphicsLevel = GraphicsQuality.IsMoved();
		QualitySettings.SetQualityLevel((int) Settings.graphicsLevel);
	}
	
	public bool IsClosed()
	{
		return Close.IsClicked();
	}
	
	public void Hide()
	{
		Background.TargetTint.a = 0;
		WindowName.TargetTint.a = 0;
		Mute.TargetTint.a = 0;
		MasterVolume.TargetTint.a = 0;
		MusicVolume.TargetTint.a = 0;
		EffectsVolume.TargetTint.a = 0;
		GraphicsQuality.TargetTint.a = 0;
		UseWasd.TargetTint.a = 0;
		UseMouse.TargetTint.a = 0;
		Close.TargetTint.a = 0;
	}
	
	public void Show()
	{
		Background.TargetTint.a = 1;
		WindowName.TargetTint.a = 1;
		Mute.TargetTint.a = 1;
		MasterVolume.TargetTint.a = 1;
		MusicVolume.TargetTint.a = 1;
		EffectsVolume.TargetTint.a = 1;
		GraphicsQuality.TargetTint.a = 1;
		UseWasd.TargetTint.a = 1;
		UseMouse.TargetTint.a = 1;
		Close.TargetTint.a = 1;
	}
	
	public override void DrawMe()
	{
		base.DrawMe();
	}
	
	public override void Tween()
	{
		base.Tween();
		
		Mute.Tween();
		MasterVolume.Tween();
		MusicVolume.Tween();
		EffectsVolume.Tween();
		UseWasd.Tween();
		UseMouse.Tween();
		GraphicsQuality.Tween();
		
		Close.Tween();
	}
	
	#endregion Methods
}
