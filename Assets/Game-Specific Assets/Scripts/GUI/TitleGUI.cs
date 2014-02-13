using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TitleGUI : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public bool DrawElements = true;
	
	public string NewGameScene;
	public Vector3 NewGameTransform;
	
	public MainForm MainForm;
	public SettingsForm SettingsForm;
	public CreditsForm CreditsForm;
	public InstructionsForm InstructionsForm;
	
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
		CreditsForm.Initialize(_maestro);
		InstructionsForm.Initialize(_maestro);
		
		MainForm.SetVisibility(true);
		SettingsForm.SetVisibility(false);
		CreditsForm.SetVisibility(false);
		InstructionsForm.SetVisibility(false);
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

			case MainForm.Feedback.Instructions:
				MainForm.SetVisibility(false);
				InstructionsForm.SetVisibility(true);
				break;

			case MainForm.Feedback.Credits:
				MainForm.SetVisibility(false);
				CreditsForm.SetVisibility(true);
				break;

			case MainForm.Feedback.Support:
				Application.ExternalEval("window.open('http://www.patreon.com/Asvarduil');");
				break;
			
			case MainForm.Feedback.NewGame:
				MainForm.SetVisibility(false);
				NewGame();
				break;
			
			case MainForm.Feedback.LoadGame:
				MainForm.SetVisibility(false);
				if(_saveFileAccess.LoadGameState())
				{
					if(DebugMode)
						Debug.Log("Game state loaded.  Transitioning to scene...");

					_transition.ChangeScenes();
				}
				else
				{
					if(DebugMode)
						Debug.LogWarning("Unable to load game state!  Starting a new game.");

					NewGame();
				}
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

		CreditsForm.DrawMe();
		switch(CreditsForm.FormResult)
		{
			case CreditsForm.Feedback.Back:
				CreditsForm.SetVisibility(false);
				MainForm.SetVisibility(true);
				break;

			default:
				break;
		}

		InstructionsForm.DrawMe();
		switch(InstructionsForm.FormResult)
		{
			case InstructionsForm.Feedback.Back:
				InstructionsForm.SetVisibility(false);
				MainForm.SetVisibility(true);
				break;

			default:
				break;
		}
	}
	
	public void FixedUpdate()
	{
		MainForm.Tween();
		SettingsForm.Tween();
		CreditsForm.Tween();
		InstructionsForm.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void NewGame()
	{
		ResetStats();
		ResetItems();
		ResetPhases();

		_transition.PrepareTransition(NewGameTransform, Vector3.zero, NewGameScene);
		_transition.ChangeScenes();
	}

	private void ResetStats()
	{
		_ambassador.MaxHP = 8;
		_ambassador.Damage = 1;
	}

	private void ResetItems()
	{
		_ambassador.ItemList = new List<ObtainableItem>();
	}

	private void ResetPhases()
	{
		_ambassador.SequenceCounters = new List<SequenceCounter>();

		SequenceCounter main = new SequenceCounter {
			Name = "Main",
			Phase = 0,
			QuestTitle = "Main Quest",
			QuestDetails = "Talk to King Aylea XXXIII"
		};

		SequenceCounter side = new SequenceCounter {
			Name = "Goldensage",
			Phase = 0,
			QuestTitle = "Side Quest",
			QuestDetails = "Talk to everyone to find the side quest."
		};

		_ambassador.SequenceCounters.Add(main);
		_ambassador.SequenceCounters.Add(side);
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
		LoadGame,
		Credits,
		Instructions,
		Support
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
	public AsvarduilButton SupportThisGameButton;
	public AsvarduilImageButton InstructionButton;
	public AsvarduilImageButton CreditsButton;
	
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

			if(_creditsClicked)
				result = Feedback.Credits;

			if(_instructionsClicked)
				result = Feedback.Instructions;

			if(_supportGameClicked)
				result = Feedback.Support;
			
			return result;
		}
	}
	
	private bool _settingsClicked = false;
	private bool _newGameClicked = false;
	private bool _loadGameClicked = false;
	private bool _creditsClicked = false;
	private bool _instructionsClicked = false;
	private bool _supportGameClicked = false;
	
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
		SupportThisGameButton.TargetTint.a = opacity;
		InstructionButton.TargetTint.a = opacity;
		CreditsButton.TargetTint.a = opacity;
	}
	
	public override void DrawMe()
	{
		GUI.skin = Skin;
		
		SplashBackground.DrawMe();
		TitlePane.DrawMe();
		
		_settingsClicked = SettingsButton.IsClicked();
		_loadGameClicked = LoadGameButton.IsClicked();
		_newGameClicked = NewGameButton.IsClicked();
		_creditsClicked = CreditsButton.IsClicked();
		_instructionsClicked = InstructionButton.IsClicked();
		_supportGameClicked = SupportThisGameButton.IsClicked();
		
		if(_settingsClicked
		   || _loadGameClicked
		   || _newGameClicked
		   || _creditsClicked
		   || _supportGameClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public override void Tween()
	{
		SplashBackground.Tween();
		TitlePane.Tween();
		SettingsButton.Tween();
		NewGameButton.Tween();
		LoadGameButton.Tween();
		CreditsButton.Tween();
		InstructionButton.Tween();
		SupportThisGameButton.Tween();
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

		AudioEnabledCheckbox.Value = Settings.soundEnabled;
		MasterVolume.Value = Settings.soundEnabled ? Settings.masterVolume : 0.0f;
		MusicVolume.Value = Settings.musVolume;
		EffectsVolume.Value = Settings.sfxVolume;
		GraphicsQuality.Value = QualitySettings.GetQualityLevel();
	}

	public void SaveSettings()
	{
		Settings.soundEnabled = AudioEnabledCheckbox.Value;
		Settings.masterVolume = Settings.soundEnabled ? MasterVolume.Value : 0.0f;
		Settings.musVolume = MusicVolume.Value;
		Settings.sfxVolume = EffectsVolume.Value;

		if(DebugMode)
			Debug.Log("(Save) State of Settings static object:\r\n"
		    	      + "Sound Enabled? " + Settings.soundEnabled + "\r\n"
		        	  + "Master Volume: " + Settings.masterVolume + "\r\n"
		              + "Music Volume: " + Settings.musVolume + "\r\n"
			          + "Effects Volume: "  + Settings.sfxVolume);
	}

	public void LoadSettings()
	{
		if(DebugMode)
			Debug.Log("(Load) State of Settings static object:\r\n"
		    	      + "Sound Enabled? " + Settings.soundEnabled + "\r\n"
			          + "Master Volume: " + Settings.masterVolume + "\r\n"
	                  + "Music Volume: " + Settings.musVolume + "\r\n"
					  + "Effects Volume: "  + Settings.sfxVolume);

		AudioEnabledCheckbox.Value = Settings.soundEnabled;
		MasterVolume.Value = Settings.masterVolume;
		MusicVolume.Value = Settings.musVolume;
		EffectsVolume.Value = Settings.sfxVolume;
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

		//Settings.soundEnabled = AudioEnabledCheckbox.IsClicked();
		if(AudioEnabledCheckbox.IsClicked())
		{
			Settings.soundEnabled = AudioEnabledCheckbox.Value;
		}

		Settings.masterVolume = Settings.soundEnabled 
			                    ? MasterVolume.IsMoved()
				                : 0.0f;
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
public class InstructionsForm : AsvarduilForm
{
	#region Enumerations

	public enum Feedback
	{
		None,
		Back
	}

	#endregion Enumerations

	#region Variables / Properties

	public GUISkin Skin;
	public AudioClip ButtonSound;

	public AsvarduilImage WorldMapPane;
	public AsvarduilImage SidescrollingPane;
	public AsvarduilButton BackButton;

	private Maestro _maestro;
	private bool _backClicked;

	public Feedback FormResult
	{
		get
		{
			if(_backClicked)
				return Feedback.Back;

			return Feedback.None;
		}
	}

	#endregion Variables / Properties

	#region Constructors

	public InstructionsForm(AsvarduilImage bg, AsvarduilLabel header)
		: base(bg, header)
	{
	}

	#endregion Constructors

	#region Overrides

	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}

	public void SetVisibility(bool isVisible)
	{
		float opacity = isVisible ? 1.0f : 0.0f;

		Background.TargetTint.a = opacity;
		WorldMapPane.TargetTint.a = opacity;
		SidescrollingPane.TargetTint.a = opacity;
		BackButton.TargetTint.a = opacity;
	}

	public override void Tween ()
	{
		Background.Tween();
		WorldMapPane.Tween();
		SidescrollingPane.Tween();
		BackButton.Tween();
	}

	public override void DrawMe()
	{
		GUI.skin = Skin;

		Background.DrawMe();
		WorldMapPane.DrawMe();
		SidescrollingPane.DrawMe();

		_backClicked = BackButton.IsClicked();
		if(_backClicked)
		{
			_maestro.PlaySoundEffect(ButtonSound);
		}
	}

	#endregion Overrides
}

[Serializable]
public class CreditsForm : AsvarduilForm
{
	#region Enumerations

	public enum Feedback
	{
		None,
		Back
	}

	#endregion Enumerations

	#region Constructors

	public CreditsForm(AsvarduilImage bg, AsvarduilLabel header)
		: base(bg, header)
	{
	}

	#endregion Constructors

	#region Variables / Properties

	public GUISkin skin;
	public AudioClip buttonSound;

	public AsvarduilButton BackButton;
	public AsvarduilLabel ThanksHeader;
	public AsvarduilLabel ThanksContributors;
	public AsvarduilLabel SpecialThanksHeader;
	public AsvarduilLabel SpecialThanksContributors;

	private bool _backClicked;
	private Maestro _maestro;

	public Feedback FormResult
	{
		get
		{
			if(_backClicked)
				return Feedback.Back;

			return Feedback.None;
		}
	}

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
		BackButton.TargetTint.a = opacity;
		ThanksHeader.TargetTint.a = opacity;
		ThanksContributors.TargetTint.a = opacity;
		SpecialThanksHeader.TargetTint.a = opacity;
		SpecialThanksContributors.TargetTint.a = opacity;
	}

	public override void DrawMe()
	{
		GUI.skin = skin;

		Background.DrawMe();
		ThanksHeader.DrawMe();
		ThanksContributors.DrawMe();
		SpecialThanksHeader.DrawMe();
		SpecialThanksContributors.DrawMe();

		_backClicked = BackButton.IsClicked();
		if(_backClicked)
		{
			_maestro.PlaySoundEffect(buttonSound);
		}
	}

	public override void Tween()
	{
		Background.Tween();
		ThanksHeader.Tween();
		ThanksContributors.Tween();
		SpecialThanksHeader.Tween();
		SpecialThanksContributors.Tween();
		BackButton.Tween();
	}

	#endregion Methods
}