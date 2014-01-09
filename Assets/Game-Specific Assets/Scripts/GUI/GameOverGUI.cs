using UnityEngine;
using System.Collections;

public class GameOverGUI : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public GUISkin Skin;
	public AsvarduilImage YouWinPane;
	public AsvarduilButton TitleButton;

	private Fader _fader;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Start()
	{
		_fader = (Fader) GameObject.FindObjectOfType(typeof(Fader));
	}

	public void OnGUI()
	{
		GUI.skin = Skin;

		YouWinPane.DrawMe();

		if(TitleButton.IsClicked())
		{
			StartCoroutine(ReturnToTitle());
		}
	}

	public void Update()
	{
		YouWinPane.Tween();
		TitleButton.Tween();
	}

	#endregion Engine Hooks

	#region Methods

	private IEnumerator ReturnToTitle()
	{
		_fader.FadeOut();
		while(_fader.ScreenShown)
			yield return 0;

		Application.LoadLevel("Title");
	}

	#endregion Methods
}
