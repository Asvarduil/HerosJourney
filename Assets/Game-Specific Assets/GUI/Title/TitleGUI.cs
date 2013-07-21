using UnityEngine;
using System.Collections;

public class TitleGUI : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DrawElements = true;
	public AudioClip ButtonSound;
	public AsvarduilImage SplashBackground;
	public AsvarduilImage TitlePane;
	public AsvarduilImageButton SettingsButton;
	public AsvarduilButton NewGameButton;
	public AsvarduilButton LoadGameButton;
	
	private Maestro _maestro;
	private Ambassador _ambassador;
	private TransitionManager _transition;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
		_ambassador = Ambassador.Instance;
		_transition = TransitionManager.Instance;
	}
	
	public void OnGUI()
	{
		if(! DrawElements)
			return;
		
		SplashBackground.DrawMe();
		TitlePane.DrawMe();
		
		if(SettingsButton.IsClicked())
		{
			_maestro.PlaySoundEffect(ButtonSound);
		}
		
		if(NewGameButton.IsClicked())
		{
			_maestro.PlaySoundEffect(ButtonSound);
		}
		
		if(LoadGameButton.IsClicked())
		{
			_maestro.PlaySoundEffect(ButtonSound);
		}
	}
	
	public void FixedUpdate()
	{
		SplashBackground.Tween();
		TitlePane.Tween();
		SettingsButton.Tween();
		NewGameButton.Tween();
		LoadGameButton.Tween();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	#endregion Methods
}
