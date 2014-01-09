using UnityEngine;
using System;
using System.Collections;

public class PauseGUI : MonoBehaviour 
{
	#region Variables / Properties
	
	public string TitleScene = "Title";
	
	public PauseHUD PauseHud;
	public PauseForm PauseForm;
	public SettingsForm SettingsForm;
	
	private Fader _fader;
	private Maestro _maestro;
	private Ambassador _ambassador;
	private SaveFileAccess _saveFileAccess;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = Ambassador.Instance;
		_maestro = Maestro.DetectLastInstance();
		_fader = (Fader) FindObjectOfType(typeof(Fader));
		_saveFileAccess = _ambassador.gameObject.GetComponent<SaveFileAccess>();
		
		PauseHud.Initialize(_maestro);
		PauseForm.Initialize(_maestro);
		SettingsForm.Initialize(_maestro);
	}
	
	public void OnGUI()
	{
		PauseHud.DrawMe();
		switch(PauseHud.FormResult)
		{
			case PauseHUD.Feedback.PauseGame:
				RadiatePauseCommand();
				PauseHud.SetVisibility(false);
				PauseForm.SetVisibility(true);
				break;
		}
		
		PauseForm.DrawMe();
		switch(PauseForm.FormResult)
		{
			case PauseForm.Feedback.Resume:
				RadiateResumeCommand();
				PauseForm.SetVisibility(false);
				PauseHud.SetVisibility(true);
				break;
				
			case PauseForm.Feedback.Settings:
				SettingsForm.LoadSettings();
				SettingsForm.SetVisibility(true);
				PauseForm.SetVisibility(false);
				break;
				
			case PauseForm.Feedback.SaveToTitle:
				_saveFileAccess.SaveGameState();
				StartCoroutine(FadeToTitleScene());
				break;
		}
		
		SettingsForm.DrawMe();
		switch(SettingsForm.FormResult)
		{
			case SettingsForm.Feedback.Back:
				SettingsForm.SaveSettings();
				SettingsForm.SetVisibility(false);
				PauseForm.SetVisibility(true);
				break;
		}
	}
	
	public void FixedUpdate()
	{
		PauseHud.Tween();
		PauseForm.Tween();
		SettingsForm.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void RadiatePauseCommand()
	{
		GameObject[] allObjects = (GameObject[]) FindObjectsOfType(typeof(GameObject));
		
		foreach(var current in allObjects)
		{
			current.SendMessage("PauseThisEntity", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private void RadiateResumeCommand()
	{
		GameObject[] allObjects = (GameObject[]) FindObjectsOfType(typeof(GameObject));
		
		foreach(var current in allObjects)
		{
			current.SendMessage("ResumeThisEntity", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private IEnumerator FadeToTitleScene()
	{
		_fader.FadeOut();
		
		while(_fader.ScreenShown)
			yield return 0;
		
		Application.LoadLevel(TitleScene);
	}
	
	#endregion Methods
}

[Serializable]
public class PauseHUD 
{
	#region Enumerations
	
	public enum Feedback
	{
		None,
		PauseGame
	}
	
	#endregion Enumerations
	
	#region Variables / Properties
	
	public GUISkin Skin;
	public AudioClip ButtonSound;
	
	public AsvarduilImageButton PauseButton;
	
	public Feedback FormResult
	{
		get
		{
			if(_pauseClicked)
				return Feedback.PauseGame;
			
			return Feedback.None;
		}
	}
	
	private bool _pauseClicked;
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}
	
	public void SetVisibility(bool visible)
	{
		float visibility = visible ? 1.0f : 0.0f;
		
		PauseButton.TargetTint.a = visibility;
	}
	
	public void DrawMe()
	{
		GUI.skin = Skin;
		
		_pauseClicked = PauseButton.IsClicked();
		
		if(_pauseClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public void Tween()
	{
		PauseButton.Tween();
	}
	
	#endregion Methods
}

[Serializable]
public class PauseForm : AsvarduilForm
{
	#region Enumerations
	
	public enum Feedback
	{
		None,
		Resume,
		SaveToTitle,
		Settings
	}
	
	#endregion Enumerations
	
	#region Variables / Properties
	
	public GUISkin Skin;
	public AudioClip ButtonSound;
	
	public float VisibleBackgroundAlpha = 0.8f;
	public AsvarduilButton ResumeButton;
	public AsvarduilButton SaveToTitleButton;
	public AsvarduilButton SettingsButton;
	
	public Feedback FormResult
	{
		get
		{
			if(_resumeClicked)
				return Feedback.Resume;
			
			if(_settingsClicked)
				return Feedback.Settings;
			
			if(_saveToTitleClicked)
				return Feedback.SaveToTitle;
			
			return Feedback.None;
		}
	}
	
	private bool _resumeClicked;
	private bool _settingsClicked;
	private bool _saveToTitleClicked;
	
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public PauseForm(AsvarduilImage background, AsvarduilLabel label) 
		: base(background, label)
	{
	}
	
	#endregion Constructor
	
	#region Methods
	
	public void Initialize(Maestro maestro)
	{
		_maestro = maestro;
	}
	
	public void SetVisibility(bool visible)
	{
		float visibility = visible ? 1.0f : 0.0f;
		
		Background.TargetTint.a = visible ? VisibleBackgroundAlpha : 0.0f;
		ResumeButton.TargetTint.a = visibility;
		SaveToTitleButton.TargetTint.a = visibility;
		SettingsButton.TargetTint.a = visibility;
	}
	
	public override void DrawMe()
	{
		GUI.skin = Skin;
		Background.DrawMe();
		
		_resumeClicked = ResumeButton.IsClicked();
		_settingsClicked = SettingsButton.IsClicked();
		_saveToTitleClicked = SaveToTitleButton.IsClicked();
		
		if(_settingsClicked
		   || _resumeClicked
		   || _saveToTitleClicked)
			_maestro.PlaySoundEffect(ButtonSound);
	}
	
	public override void Tween()
	{
		Background.Tween();
		
		ResumeButton.Tween();
		SaveToTitleButton.Tween();
		SettingsButton.Tween();
	}
	
	#endregion Methods
}
