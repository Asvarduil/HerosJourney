using UnityEngine;
using System;
using System.Collections;

public class TitleGUI : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DrawElements = true;
	
	public string NewGameScene;
	public Vector3 NewGameTransform;
	
	public MainForm MainForm;
	public SettingsForm SettingsForm;
	public LoadGameForm LoadGameForm;
	
	// Private elements
	private Maestro _maestro;
	private Ambassador _ambassador;
	private SaveFileAccess _saveFileAccess;
	private TransitionManager _transition;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
		_ambassador = Ambassador.Instance;
		_transition = TransitionManager.Instance;
		_saveFileAccess = _ambassador.gameObject.GetComponent<SaveFileAccess>();
		
		MainForm.Initialize(_maestro);
		SettingsForm.Initialize(_maestro);
		LoadGameForm.Initialize(_maestro);
		
		MainForm.SetVisibility(true);
		SettingsForm.SetVisibility(false);
		LoadGameForm.SetVisibility(false);
	}
	
	public void OnGUI()
	{
		if(! DrawElements)
			return;
		
		MainForm.DrawMe();
		switch(MainForm.FormResult)
		{
			case MainForm.Feedback.Settings:
				MainForm.SetVisibility(false);
				SettingsForm.SetVisibility(true);
				break;
			
			case MainForm.Feedback.NewGame:
				MainForm.SetVisibility(false);
				NewGame();
				break;
			
			case MainForm.Feedback.LoadGame:
				MainForm.SetVisibility(false);
				LoadGameForm.SetVisibility(true);
				break;
			
			default:
				break;
		}
		
		SettingsForm.DrawMe();		
		switch(SettingsForm.FormResult)
		{
			case SettingsForm.Feedback.Back:
				SettingsForm.SetVisibility(false);
				MainForm.SetVisibility(true);
				break;
			
			default:
				break;
		}
		
		LoadGameForm.DrawMe();
		switch(LoadGameForm.FormResult)
		{
			case LoadGameForm.Feedback.Back:
				LoadGameForm.SetVisibility(false);
				MainForm.SetVisibility(true);
				break;
			
			case LoadGameForm.Feedback.Load:
				LoadGameForm.SetVisibility(false);
				if(_saveFileAccess.LoadFileIntoAmbassador())
				{
					_transition.ChangeScenes();
				}
				else
				{
					NewGame();
				}
				break;
			
			default:
				break;
		}
	}
	
	public void FixedUpdate()
	{
		MainForm.Tween();
		SettingsForm.Tween();
		LoadGameForm.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void NewGame()
	{
		_transition.PrepareTransition(NewGameTransform, Vector3.zero, NewGameScene);
		_transition.ChangeScenes();
	}
	
	#endregion Methods
}

[Serializable]
public class MainForm : AsvarduilForm
{
	#region Enumerations
	
	public enum Feedback
	{
		None,
		Settings,
		NewGame,
		LoadGame
	}
			
	#endregion Enumerations
	
	#region Constructor
	
	public MainForm(AsvarduilImage background, AsvarduilLabel label) 
		: base(background, label)
	{
	}
	
	#endregion Constructor
			
	#region Variables / Properties
	
	public GUISkin Skin;
	public AudioClip ButtonSound;
	
	public AsvarduilImage SplashBackground;
	public AsvarduilImage TitlePane;
	public AsvarduilImageButton SettingsButton;
	public AsvarduilButton NewGameButton;
	public AsvarduilButton LoadGameButton;
	
	public Feedback FormResult
	{
		get
		{
			Feedback result = Feedback.None;
			
			if(_settingsClicked)
				result = Feedback.Settings;
			
			if(_newGameClicked)
				result = Feedback.NewGame;
			
			if(_loadGameClicked)
				result = Feedback.LoadGame;
			
			return result;
		}
	}
	
	private bool _settingsClicked = false;
	private bool _newGameClicked = false;
	private bool _loadGameClicked = false;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}
	
	public void SetVisibility(bool visible)
	{
		float opacity = visible ? 1.0f : 0.0f;

		SplashBackground.TargetTint.a = opacity;
		TitlePane.TargetTint.a = opacity;
		SettingsButton.TargetTint.a = opacity;
		NewGameButton.TargetTint.a = opacity;
		LoadGameButton.TargetTint.a = opacity;
	}
	
	public override void DrawMe()
	{
		GUI.skin = Skin;
		
		SplashBackground.DrawMe();
		TitlePane.DrawMe();
		
		_settingsClicked = SettingsButton.IsClicked();
		_loadGameClicked = LoadGameButton.IsClicked();
		_newGameClicked = NewGameButton.IsClicked();
		
		if(_settingsClicked
		   || _loadGameClicked
		   || _newGameClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public override void Tween()
	{
		SplashBackground.Tween();
		TitlePane.Tween();
		SettingsButton.Tween();
		NewGameButton.Tween();
		LoadGameButton.Tween();
	}
	
	#endregion Methods
}

[Serializable]
public class SettingsForm : AsvarduilForm
{
	#region Enumerations
	
	public enum Feedback
	{
		None,
		Back
	}
	
	#endregion Enumerations
	
	#region Constructor
	
	public SettingsForm(AsvarduilImage background, AsvarduilLabel label) 
		: base(background, label)
	{
	}
	
	#endregion Constructor
	
	#region Variables / Properties
	
	public GUISkin Skin;
	public AudioClip ButtonSound;
	
	public AsvarduilCheckbox AudioEnabledCheckbox;
	public AsvarduilSlider MasterVolume;
	public AsvarduilSlider MusicVolume;
	public AsvarduilSlider EffectsVolume;
	public AsvarduilSlider GraphicsQuality;
	
	public AsvarduilButton BackButton;
	
	public Feedback FormResult
	{
		get
		{
			Feedback result = Feedback.None;
			if(_backClicked)
				result = Feedback.Back;
			
			return result;
		}
	}
	
	private bool _backClicked = false;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}
	
	public void SetVisibility(bool visible)
	{
		float opacity = visible ? 1.0f : 0.0f;
		
		Background.TargetTint.a = opacity;
		WindowName.TargetTint.a = opacity;
		BackButton.TargetTint.a = opacity;
		AudioEnabledCheckbox.TargetTint.a = opacity;
		MasterVolume.TargetTint.a = opacity;
		MasterVolume.Label.TargetTint.a = opacity;
		MusicVolume.TargetTint.a = opacity;
		MusicVolume.Label.TargetTint.a = opacity;
		EffectsVolume.TargetTint.a = opacity;
		EffectsVolume.Label.TargetTint.a = opacity;
		GraphicsQuality.TargetTint.a = opacity;
		GraphicsQuality.Label.TargetTint.a = opacity;
	}
	
	public override void DrawMe()
	{
		GUI.skin = Skin;
		
		Background.DrawMe();
		WindowName.DrawMe();
		Settings.soundEnabled = AudioEnabledCheckbox.IsClicked();
		Settings.masterVolume = MasterVolume.IsMoved();
		Settings.musVolume = MusicVolume.IsMoved();
		Settings.sfxVolume = EffectsVolume.IsMoved();
		QualitySettings.SetQualityLevel((int) GraphicsQuality.IsMoved());
		
		_backClicked = BackButton.IsClicked();
		
		if(_backClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public override void Tween()
	{
		Background.Tween();
		WindowName.Tween();
		BackButton.Tween();
		AudioEnabledCheckbox.Tween();
		MasterVolume.Tween();
		MusicVolume.Tween();
		EffectsVolume.Tween();
		GraphicsQuality.Tween();
	}
	
	#endregion Methods
}
	
[Serializable]
public class LoadGameForm : AsvarduilForm
{
	#region Enumeration
	
	public enum Feedback
	{
		None,
		Load,
		Back
	}
	
	#endregion Enumerations
	
		#region Constructor
	
	public LoadGameForm(AsvarduilImage background, AsvarduilLabel label) 
		: base(background, label)
	{
	}
	
	#endregion Constructor
	
	#region Variables / Properties
	
	public GUISkin Skin;
	public AudioClip ButtonSound;
	
	public AsvarduilButton BackButton;
	public AsvarduilButton LoadButton;
	
	public Feedback FormResult
	{
		get
		{
			Feedback result = Feedback.None;
			if(_backClicked)
				result = Feedback.Back;
			
			return result;
		}
	}
	
	private bool _loadClicked = false;
	private bool _backClicked = false;
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}
	
	public void SetVisibility(bool visible)
	{
		BackButton.TargetTint.a = visible ? 1.0f : 0.0f;
		LoadButton.TargetTint.a = visible ? 1.0f : 0.0f;
		Background.TargetTint.a = visible ? 1.0f : 0.0f;
	}
	
	public override void DrawMe()
	{	
		GUI.skin = Skin;
		Background.DrawMe();
		
		_backClicked = BackButton.IsClicked();
		_loadClicked = LoadButton.IsClicked();
		
		if(_backClicked
		   || _loadClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public override void Tween()
	{
		Background.Tween();
		BackButton.Tween();
		LoadButton.Tween();
	}
	
	#endregion Methods
}